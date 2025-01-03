using W88.TeleBot.ServiceBot.Constants;
using W88.TeleBot.ServiceBot.Interfaces;

namespace W88.TeleBot.ServiceBot.Commands.Common;

public class CancelCommand(ICommandHandler commandHandler, IUserStateManager userStateManager)
    : BaseCommand(commandHandler,
        userStateManager)
{
    public override string Name => CommandNames.CancelCommand;
    public override string CommandText => TextCommands.Cancel;

    public override async Task ExecuteAsync(ITelegramBotClient botClient, Message? message,
        CallbackQuery? callbackQuery, InlineQuery? inlineQuery, CancellationToken cancellationToken)
    {
        var chatId = message?.Chat.Id;
        if (chatId == null) return;

        var messageId = message?.MessageId ?? -1;

        const string textMsg = "Action cancelled.";

        _ = await botClient.EditMessageText(
            chatId: chatId,
            messageId: messageId,
            text: textMsg,
            replyMarkup: null,
            cancellationToken: cancellationToken);
    }
}