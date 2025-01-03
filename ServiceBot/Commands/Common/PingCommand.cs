using CW88.TeleBot.ServiceBot.Constants;
using CW88.TeleBot.ServiceBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CW88.TeleBot.ServiceBot.Commands.Common;

public class PingCommand(ICommandHandler commandHandler, IUserStateManager userStateManager)
    : BaseCommand(commandHandler,
        userStateManager)
{
    public override string Name => CommandNames.PingCommand;
    public override string CommandText => TextCommands.Ping;

    public override async Task ExecuteAsync(ITelegramBotClient botClient, Message? message,
        CallbackQuery? callbackQuery, InlineQuery? inlineQuery, CancellationToken cancellationToken)
    {
            var chatId = message?.Chat.Id;
            if (chatId == null) return;

            _ = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Pong!",
                replyMarkup: null,
                cancellationToken: cancellationToken);
        }
}