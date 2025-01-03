using CW88.TeleBot.Model.Player;
using CW88.TeleBot.Resources;
using CW88.TeleBot.ServiceBot.Constants;
using CW88.TeleBot.ServiceBot.Interfaces;
using CW88.TeleBot.Services.Interfaces;
using Microsoft.Extensions.Localization;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CW88.TeleBot.ServiceBot.Commands;

public class ShareContactCommand(
    IUserStateManager userStateManager,
    ICommandHandler commandHandler,
    IPlayerService playerService,
    ILogger<ShareContactCommand> logger,
    IStringLocalizer<SharedResource> localizer)
    : BaseCommand(commandHandler, userStateManager)
{
    public override string Name => CommandNames.ShareContactCommand;
    public override string[] Commands => [TextCommands.ShareContact, TextCommands.ConfirmRegistration];

    public override async Task ExecuteAsync(ITelegramBotClient botClient, Message? message,
        CallbackQuery? callbackQuery, InlineQuery? inlineQuery, CancellationToken cancellationToken)
    {
        var chatId = message?.Chat.Id;
        if (chatId == null) return;

        try
        {
            var userContact = message?.Contact;
            if (userContact == null) return;

            var languageCode = message?.From?.LanguageCode;
            if (languageCode == null) return;

            SetCulture(languageCode);

            var formatPhoneNumber = userContact.PhoneNumber.Replace("+", "");
            if (formatPhoneNumber.StartsWith('0'))
            {
                formatPhoneNumber = '6' + formatPhoneNumber;
            }

            // var playerResponse = await playerService.GetPlayerByTelegramUID(chatId.ToString(), formatPhoneNumber);
            // if (playerResponse.Succeeded)
            // {
            //     var playCommand = commandHandler.GetBotCommand(commandName: CommandNames.PlayCommand);
            //     if (playCommand != null)
            //     {
            //         await playCommand.ExecuteAsync(botClient, message, null, null, cancellationToken);
            //     }
            // }
            // else
            // {
                // Handle to update phone number with TelegramChatId
                // var registerPlayerResult = await playerService.RegisterPlayer(new RegisterPlayerArgs
                // {
                //     MobileNumber = formatPhoneNumber,
                //     TelegramUID = chatId.ToString()!
                // });

                // if (!registerPlayerResult.Succeeded)
                // {
                //     var errorMessage = registerPlayerResult.Error.Code switch
                //     {
                //         "EPS_REP_IMV_0100" => localizer["You have entered an invalid mobile number. Please try again."],
                //         "EPS_REP_IMV_0101" => localizer["You have entered a registered mobile number. Please try again."],
                //         _ =>
                //             $"{localizer["Unable to register:"]} {registerPlayerResult.Error.Code}. {localizer["Please contact Customer Service if problem persists."]}"
                //     };
                //
                //     await botClient.SendTextMessageAsync(chatId,
                //         errorMessage,
                //         cancellationToken: cancellationToken);
                // }
                // else
                // {
                //     var playCommand = commandHandler.GetBotCommand(commandName: CommandNames.PlayCommand);
                //     if (playCommand != null)
                //     {
                //         await playCommand.ExecuteAsync(botClient, message, null, null, cancellationToken);
                //     }
                // }
            //}
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            await botClient.SendTextMessageAsync(chatId,
                $"An error occured. Please try again later!",
                cancellationToken: cancellationToken);
        }
    }
}