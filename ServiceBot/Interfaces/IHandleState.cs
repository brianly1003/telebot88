using Telegram.Bot;
using Telegram.Bot.Types;

namespace CW88.TeleBot.ServiceBot.Interfaces;

public interface IHandleState
{
    Task HandleResponse(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken);
}