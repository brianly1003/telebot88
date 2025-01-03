using Telegram.Bot.Polling;

namespace W88.TeleBot.Services.Interfaces;

public interface IBaseUpdateHandler : IUpdateHandler, IWebhookHandler
{
    Task ConfigBot(ITelegramBotClient botClient, CancellationToken cancellationToken);
}