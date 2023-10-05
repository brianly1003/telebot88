using Telegram.Bot.Types;
using Telegram.Bot;

namespace TeleBot.ServiceBot.Interfaces
{
    public interface IHandleState
    {
        Task HandleResponse(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken);
    }
}
