namespace W88.TeleBot.Services.Interfaces
{
    public interface ITelegramBotClientFactory
    {
        ITelegramBotClient GetClient(string botName);
    }
}
