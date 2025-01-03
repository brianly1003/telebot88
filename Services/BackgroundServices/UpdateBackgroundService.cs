using CW88.TeleBot.Services.Interfaces;
using CW88.TeleBot.Services.Queues;

namespace CW88.TeleBot.Services.BackgroundServices;

public class UpdateBackgroundService(
    ILogger<UpdateBackgroundService> logger,
    IServiceProvider serviceProvider,
    UpdateQueue updateQueue)
    : BackgroundService
{
    private readonly SemaphoreSlim _parallelSemaphore = new(10); // Max 10 concurrent tasks

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var tasks = new List<Task>();

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (updateQueue.TryDequeue(out var item))
                {
                    var (botName, update) = item;

                    await _parallelSemaphore.WaitAsync(stoppingToken); // Limit concurrent tasks

                    var task = Task.Run(async () =>
                    {
                        using var scope = serviceProvider.CreateScope();
                        try
                        {
                            // Resolve ITelegramBotClient for the bot
                            var botClientFactory = scope.ServiceProvider.GetRequiredService<ITelegramBotClientFactory>();
                            var botClient = botClientFactory.GetClient(botName);

                            // Resolve IBaseUpdateHandler
                            var updateHandler = scope.ServiceProvider.GetRequiredService<IBaseUpdateHandler>();

                            // Process the update
                            await updateHandler.HandleUpdateAsync(botClient, update, stoppingToken);

                            // Mark the update as processed
                            await updateQueue.MarkAsProcessedAsync(update.Id);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Error processing update for bot '{BotName}': {UpdateId}", botName, update.Id);
                        }
                        finally
                        {
                            _parallelSemaphore.Release(); // Release the semaphore slot
                        }
                    }, stoppingToken);

                    tasks.Add(task);
                }
                else
                {
                    await Task.Delay(100, stoppingToken); // No updates in the queue, wait briefly
                }

                // Remove completed tasks to prevent memory growth
                tasks.RemoveAll(t => t.IsCompleted);
            }
        }
        catch (OperationCanceledException)
        {
            // Gracefully handle shutdown signal
            logger.LogInformation("Background service is stopping.");
        }
        finally
        {
            // Wait for all tasks to complete before shutdown
            await Task.WhenAll(tasks);
        }
    }

    //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    //{
    //    while (!stoppingToken.IsCancellationRequested)
    //    {
    //        if (updateQueue.TryDequeue(out var update))
    //        {
    //            using var scope = serviceProvider.CreateScope();
    //            var updateHandler = scope.ServiceProvider.GetRequiredService<IBaseUpdateHandler>();

    //            try
    //            {
    //                // Handle the update
    //                await updateHandler.HandleUpdateAsync(null, update, stoppingToken);

    //                // Mark the update as processed
    //                await updateQueue.MarkAsProcessedAsync(update.Id);
    //            }
    //            catch (Exception ex)
    //            {
    //                logger.LogError(ex, "Error processing update {UpdateId}", update.Id);
    //                await updateHandler.HandleErrorAsync(null!, ex, Telegram.Bot.Polling.HandleErrorSource.HandleUpdateError, stoppingToken);
    //            }
    //        }
    //        else
    //        {
    //            // Wait briefly if no updates are in the queue
    //            await Task.Delay(100, stoppingToken);
    //        }
    //    }
    //}
}