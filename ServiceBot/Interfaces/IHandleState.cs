namespace W88.TeleBot.ServiceBot.Interfaces;

public interface IHandleState
{
    Task HandleResponse(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken);
}