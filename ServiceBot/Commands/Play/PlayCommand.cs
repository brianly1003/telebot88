using CW88.TeleBot.Model;
using CW88.TeleBot.Resources;
using CW88.TeleBot.ServiceBot.Constants;
using CW88.TeleBot.ServiceBot.Interfaces;
using CW88.TeleBot.ServiceBot.Models;
using CW88.TeleBot.Services.Interfaces;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using File = System.IO.File;
using InputFile = Telegram.Bot.Types.InputFile;

namespace CW88.TeleBot.ServiceBot.Commands.Play;

public class PlayCommand(
    ICommandHandler commandHandler,
    IUserStateManager userStateManager,
    IPlayerService playerService,
    ITelegramAuth telegramAuth,
    ILogger<PlayCommand> logger,
    IOptions<CoreConfig> options,
    IStringLocalizer<SharedResource> localizer)
    : BaseCommand(commandHandler, userStateManager)
{
    private readonly CoreConfig _coreConfig = options.Value;
    private BotConfig? _botConfig;

    public override string Name => CommandNames.PlayCommand; 
    public override string CommandText => TextCommands.Play;

    public override async Task ExecuteAsync(ITelegramBotClient botClient, Message? message,
        CallbackQuery? callbackQuery, InlineQuery? inlineQuery, CancellationToken cancellationToken)
    {
        _botConfig = _coreConfig.Bots.FirstOrDefault(o => o.BotToken.StartsWith(botClient.BotId.ToString()));
        if (_botConfig == null) return;

        var chatId = message?.Chat.Id;
        if (message == null || chatId == null) return;

        try
        {
            if (callbackQuery == null)
            {
                var languageCode = message.From?.LanguageCode;
                if (languageCode == null) return;

                SetCulture(languageCode);
                if (message.Type != MessageType.Contact) return;
                var userContact = message.Contact;
                if (userContact == null) return;

                var callbackDataObject = new CallbackData(CommandName: CommandNames.PlayCommand);
                var callbackDataString = JsonConvert.SerializeObject(callbackDataObject);

                var formatPhoneNumber = userContact.PhoneNumber.Replace("+", "");
                if (formatPhoneNumber.StartsWith('0'))
                {
                    formatPhoneNumber = '6' + formatPhoneNumber;
                }

                var parameters = new Dictionary<string, string?>
                {
                    { "uid", formatPhoneNumber },
                    { "chat_id", userContact.UserId.ToString() },
                    { "first_name", userContact.FirstName },
                    { "last_name", userContact.LastName },
                    { "auth_date", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() }
                };
                var data = telegramAuth.BuildQueryString(parameters);
                var authData = $"{data}&hash={telegramAuth.EncryptData(data, _coreConfig.ValidationKey)}";
                var miniAppUrl = $"{_coreConfig.MiniAppUrl}/mini-app?{authData}";
                var chatSupportUrl = $"{_coreConfig.ChatUrl}";

                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    new[] { InlineKeyboardButton.WithCallbackData(localizer["Launch Website"], callbackDataString) },
                    [InlineKeyboardButton.WithWebApp(localizer["Launch MiniApp"], new WebAppInfo { Url = miniAppUrl })],
                    [InlineKeyboardButton.WithUrl(localizer["LiveChat Support"], new WebAppInfo { Url = chatSupportUrl })]
                });

                var htmlText = localizer["Enjoy our games provided by our exclusive providers!"];

                //var imageFile = InputFile.FromUri($"{_botConfig.MiniAppUrl}/assets/images/cw88-welcome.png");
                var currentDirectory = Directory.GetCurrentDirectory();
                var filePath = Path.Combine(currentDirectory, @"Data\Images\cw88-welcome.png");
                await using var stream = File.OpenRead(filePath);
                var imageFile = InputFile.FromStream(stream, "cw88-welcome.png");

                _ = await botClient.SendPhotoAsync(
                    chatId: chatId,
                    photo: imageFile,
                    caption: htmlText,
                    replyMarkup: inlineKeyboard,
                    parseMode: ParseMode.Html,
                    cancellationToken: cancellationToken);
            }
            else
            {
                var languageCode = callbackQuery.From.LanguageCode;
                if (languageCode == null) return;

                SetCulture(languageCode);

                var oneTimeLoginLinkResponse = await playerService.OneTimeLoginLinkFor(chatId.ToString());
                if (!oneTimeLoginLinkResponse.Succeeded)
                {
                    var errorMessage =
                        $"{localizer["Unable to get one time login link:"]} {oneTimeLoginLinkResponse.Error.Code}. {localizer["Please contact Customer Service if problem persists."]}";
                    await botClient.SendTextMessageAsync(chatId,
                        errorMessage,
                        cancellationToken: cancellationToken);
                }
                else
                {
                    _ = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: oneTimeLoginLinkResponse.Result,
                        cancellationToken: cancellationToken);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.ToString());
            await botClient.SendTextMessageAsync(chatId,
                localizer["An error occured. Please try again later!"],
                cancellationToken: cancellationToken);
        }
    }
}