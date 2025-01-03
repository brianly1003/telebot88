using CW88.TeleBot.Model;
using CW88.TeleBot.Services.Interfaces;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace CW88.TeleBot.Services.BackgroundServices;

public class TelegramBotHostedService(
    ILogger<TelegramBotHostedService> logger,
    IServiceProvider serviceProvider,
    IOptions<CoreConfig> botOptions)
    : IHostedService
{
    private readonly CoreConfig _coreConfig = botOptions.Value;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var botConfig in _coreConfig.Bots)
        {
            using var scope = serviceProvider.CreateScope();
            var botClientFactory = scope.ServiceProvider.GetRequiredService<ITelegramBotClientFactory>();
            var botClient = botClientFactory.GetClient(botConfig.BotName.ToLowerInvariant());
            var updateHandler = scope.ServiceProvider.GetRequiredService<IBaseUpdateHandler>();

            if (botConfig.UseWebhook)
            {
                var webhookAddress = $"{botConfig.HostAddress}/{botConfig.BotName.ToLowerInvariant()}";
                await botClient.SetWebhook(
                    url: webhookAddress,
                    allowedUpdates: Array.Empty<UpdateType>(),
                    secretToken: botConfig.SecretToken,
                    cancellationToken: cancellationToken);

                logger.LogInformation("Setting webhook for bot '{BotName}': {WebhookAddress}", botConfig.BotName, webhookAddress);
            }
            else
            {
                await botClient.DeleteWebhook(cancellationToken: cancellationToken);
                ReceiverOptions receiverOptions = new()
                {
                    AllowedUpdates = [],
                };

                botClient.StartReceiving(
                    updateHandler: updateHandler.HandleUpdateAsync,
                    errorHandler: updateHandler.HandleErrorAsync,
                    receiverOptions: receiverOptions,
                    cancellationToken: cancellationToken
                );

                logger.LogInformation("Starting bot '{BotName}' in polling mode", botConfig.BotName);
            }
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var botConfig in _coreConfig.Bots)
        {
            using var scope = serviceProvider.CreateScope();
            var botClientFactory = scope.ServiceProvider.GetRequiredService<ITelegramBotClientFactory>();
            var botClient = botClientFactory.GetClient(botConfig.BotName);

            await botClient.DeleteWebhook(cancellationToken: cancellationToken);
            await botClient.Close(cancellationToken: cancellationToken);
        }
    }
}