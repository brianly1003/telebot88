using CW88.TeleBot.ServiceBot.Constants;
using CW88.TeleBot.ServiceBot.Interfaces;
using System.Globalization;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CW88.TeleBot.ServiceBot;

public abstract class BaseCommand(
    ICommandHandler commandHandler,
    IUserStateManager userStateManager) : IBotCommand
{
    public abstract string Name { get; }
    public virtual string CommandText => string.Empty;
    public virtual string[] Commands => [];

    protected void SetCulture(string? languageCode)
    {
        if (string.IsNullOrEmpty(languageCode)) return;

        CultureInfo.CurrentCulture = new CultureInfo(languageCode);
        CultureInfo.CurrentUICulture = new CultureInfo(languageCode);
    }

    public virtual async Task ExecuteAsync(ITelegramBotClient botClient, Message? message, CallbackQuery? callbackQuery,
        InlineQuery? inlineQuery, CancellationToken cancellationToken)
    {
        var chatId = message?.Chat.Id;
        if (chatId == null) return;

        if (message?.Text != null)
            _ = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: message.Text,
                cancellationToken: cancellationToken);
    }

    public virtual Task HandleResponse(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public virtual async Task UserInteractive(ITelegramBotClient botClient, Message? message,
        CancellationToken cancellationToken)
    {
        var chatId = message?.Chat.Id;
        if (chatId == null) return;

        var messageText = message?.Text?.Trim().ToLower() ?? string.Empty;

        if (TextCommands.AllCommands.Contains(messageText))
        {
            await userStateManager.RemoveUserState((long)chatId);
            var nextCommand = commandHandler.GetBotCommand(commandText: messageText);
            if (nextCommand != null)
            {
                await nextCommand.ExecuteAsync(botClient, message, null, null, cancellationToken);
            }
        }
        else
        {
            await HandleResponse(botClient, message, cancellationToken);
        }
    }
}