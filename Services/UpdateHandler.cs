using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using W88.TeleBot.ServiceBot.Constants;
using W88.TeleBot.ServiceBot.Interfaces;
using W88.TeleBot.ServiceBot.Utils;
using W88.TeleBot.Services.Interfaces;

namespace W88.TeleBot.Services;

public class UpdateHandler(
    ILogger<UpdateHandler> logger,
    IUserStateManager userStateManager,
    ICommandFactory commandFactory)
    : IBaseUpdateHandler
{
    public async Task ConfigBot(ITelegramBotClient botClient, CancellationToken cancellationToken)
    {
        await botClient.DeleteMyCommands(scope: BotCommandScope.AllPrivateChats(), cancellationToken: cancellationToken);
        await botClient.SetMyCommands(commands:
        [
            new BotCommand { Command = TextCommands.Start, Description = "Start" },
            //new BotCommand { Command = TextCommands.Play, Description = "Play Game" },
            //new BotCommand { Command = TextCommands.OpenMiniApp, Description = "Open MiniApp" },
            //new BotCommand { Command = TextCommands.ChooseGames, Description = "Choose Game" },
            //new BotCommand { Command = TextCommands.InlineButton, Description = "Inline Button" }
        ], scope: BotCommandScope.Default(), cancellationToken: cancellationToken);

        const string botDescription = $"Welcome to CW88 Casino. " +
                                      $"We offer thrilling gaming experience on the web at CW88.Club and Telegram MiniApp Casino here within this BOT! " +
                                      $"Sign up and join us today, enjoy generous WELCOME BONUS and other lucrative bonuses! " +
                                      $"Start CUCIWIN88 NOW!";

        await botClient.SetMyDescription(botDescription, cancellationToken: cancellationToken);
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await (update switch
        {
            { Message: { } message } => BotOnMessageReceived(botClient, message, cancellationToken),
            { EditedMessage: { } message } => BotOnMessageReceived(botClient, message, cancellationToken),
            { CallbackQuery: { } callbackQuery } => BotOnCallbackQueryReceived(botClient, callbackQuery, cancellationToken),
            { InlineQuery: { } inlineQuery } => BotOnInlineQueryReceived(botClient, inlineQuery, cancellationToken),
            { ChosenInlineResult: { } chosenInlineResult } => BotOnChosenInlineResultReceived(botClient, chosenInlineResult, cancellationToken),
            // UpdateType.ChosenInlineResult:
            // UpdateType.ChannelPost:
            // UpdateType.EditedChannelPost:
            // UpdateType.ShippingQuery:
            // UpdateType.PreCheckoutQuery:
            // UpdateType.Poll:
            // UpdateType.PollAnswer:
            // UpdateType.MyChatMember:
            // UpdateType.ChatMember:
            // UpdateType.ChatJoinRequest:
            _ => UnknownUpdateHandlerAsync(update, cancellationToken)
        });
    }

    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source,
        CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException =>
                $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        logger.LogInformation("HandleError: {ErrorMessage}", errorMessage);

        // Cooldown in case of network connection error
        if (exception is RequestException)
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
    }
    #region Private Methods

    private async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        // Initial Loading
        await InitialLoading(botClient, message, cancellationToken);

        // Fetch bot information
        var me = await botClient.GetMe(cancellationToken);

        var chatId = message.Chat.Id;

        var result = await userStateManager.TryGetUserState(chatId);
        if (result is { IsSuccess: true, Command: not null })
        {
            var currentCommand = result.Command;
            await currentCommand.HandleResponse(botClient, message, cancellationToken);
            return;
        }

        var command = commandFactory.CreateCommand(botName: me.Username, message);
        if (command != null)
        {
            await command.ExecuteAsync(botClient, message, null, null, cancellationToken);
        }
    }

    private async Task BotOnCallbackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        // Fetch bot information
        var me = await botClient.GetMe(cancellationToken);

        var callbackDataString = callbackQuery.Data ?? string.Empty;
        var callbackDataObject = DataExtensions.BuildCallbackData(callbackDataString);

        var commandText = "";
        var commandName = "";
        if (!string.IsNullOrEmpty(callbackQuery.GameShortName))
        {
            commandText = TextCommands.LaunchGame;
        }
        else
        {
            if (callbackDataObject == null)
            {
                commandText = callbackDataString;
            }
            else
            {
                commandText = callbackDataObject.CommandText;
                commandName = callbackDataObject.CommandName;
            }
        }

        var command = commandFactory.CreateCommand(botName: me.Username, commandText: commandText,
            commandName: commandName);
        if (command != null)
        {
            await command.ExecuteAsync(botClient, callbackQuery.Message, callbackQuery, null, cancellationToken);
        }
    }

    private async Task BotOnInlineQueryReceived(ITelegramBotClient botClient, InlineQuery? inlineQuery, CancellationToken cancellationToken)
    {
        var me = await botClient.GetMe(cancellationToken);

        // TODO: Refactor
        inlineQuery!.Query = CommandNames.GamesCommand;
        var command = commandFactory.CreateCommand(me.Username, inlineQuery: inlineQuery);
        if (command != null)
        {
            await command.ExecuteAsync(botClient, null, null, inlineQuery, cancellationToken);
        }
    }

    private async Task InitialLoading(ITelegramBotClient botClient, Message? message, CancellationToken cancellationToken)
    {
        if (message == null || message.Chat.Type != ChatType.Private)
            return;

        var chatId = message.Chat.Id;

        await botClient.SetChatMenuButton(chatId: chatId, menuButton: new MenuButtonCommands(),
            cancellationToken: cancellationToken);
    }
    #endregion

    #region Inline Mode

    private async Task BotOnChosenInlineResultReceived(ITelegramBotClient botClient, ChosenInlineResult chosenInlineResult,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Received inline result: {ChosenInlineResultId}", chosenInlineResult.ResultId);

        await botClient.SendMessage(
            chatId: chosenInlineResult.From.Id,
            text: $"You chose result with Id: {chosenInlineResult.ResultId}",
            cancellationToken: cancellationToken);
    }

    #endregion

    private Task UnknownUpdateHandlerAsync(Update update, CancellationToken cancellationToken)
    {
        logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }
}