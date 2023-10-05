using Telegram.Bot;
using Telegram.Bot.Types;

namespace TeleBot.ServiceBot.Interfaces
{
    public interface IBotCommand : IHandleState
    {
        string Name { get; }

        string CommandText { get; }

        Task ExecuteAsync(ITelegramBotClient botClient, Message? message, CallbackQuery? callbackQuery, CancellationToken cancellationToken);

        Task UserInteractive(ITelegramBotClient botClient, Message? message, CancellationToken cancellationToken);
    }
}
