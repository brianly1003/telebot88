using Microsoft.Extensions.Options;
using W88.TeleBot.Model;
using W88.TeleBot.Services.Interfaces;

namespace W88.TeleBot.Services;

public class TelegramBotClientFactory(IServiceProvider serviceProvider) : ITelegramBotClientFactory
{
    public ITelegramBotClient GetClient(string botName)
    {
        // Resolve the named HttpClient for the specific bot
        var client = serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient(botName.ToLowerInvariant());
        var coreConfig = serviceProvider.GetRequiredService<IOptions<CoreConfig>>().Value;

        // Perform a case-insensitive search for the bot configuration
        var botConfig = coreConfig.Bots.FirstOrDefault(b =>
            string.Equals(b.BotName, botName, StringComparison.InvariantCultureIgnoreCase));

        if (botConfig == null)
            throw new ArgumentException($"No configuration found for bot: {botName}");

        return new TelegramBotClient(botConfig.BotToken, client);
    }
}