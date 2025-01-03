using Telegram.Bot;
using Telegram.Bot.Polling;

namespace CW88.TeleBot.Services.Interfaces;

public interface IBaseUpdateHandler : IUpdateHandler, IWebhookHandler
{
    Task ConfigBot(ITelegramBotClient botClient, CancellationToken cancellationToken);
}