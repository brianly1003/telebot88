using CW88.TeleBot.ServiceBot.Interfaces;
using CW88.TeleBot.ServiceBot.Models;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace CW88.TeleBot.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController(
    ILogger<NotificationController> logger,
    IUserStateManager userStateManager,
    ICommandFactory commandFactory,
    ITelegramBotClient botClient)
    : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        using CancellationTokenSource cts = new();

        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = []
        };

        var me = await botClient.GetMe(cancellationToken: cts.Token);

        botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            errorHandler: HandleErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
        );

        // Send cancellation request to stop bot
        await cts.CancelAsync();

        return Ok();

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var chatId = update.Message?.Chat.Id ?? update.CallbackQuery?.Message?.Chat.Id;
            if (chatId == null) return;

            if (update.Message is { } message)
            {
                if (message.Text is { } messageText || message.Contact != null)
                {
                    await HandleMessageUpdate(botClient, update.Message, cancellationToken);
                }
            }
            else if (update.CallbackQuery is { } callbackQuery)
            {
                await HandleCallbackQueryUpdate(botClient, callbackQuery, cancellationToken);
            }
        }

        async Task HandleMessageUpdate(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            var chatId = message.Chat.Id;

            var result = await userStateManager.TryGetUserState(chatId);
            if (result is { IsSuccess: true, Command: not null })
            {
                var currentCommand = result.Command;
                await currentCommand.HandleResponse(botClient, message, cancellationToken);
                return;
            }

            var command = commandFactory.CreateCommand(me.Username, message);
            if (command != null)
            {
                await command.ExecuteAsync(botClient, message, null, null, cancellationToken);
            }
        }

        async Task HandleCallbackQueryUpdate(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            var callbackData = callbackQuery.Data;
            var command = commandFactory.CreateCommand(me.Username, commandText: callbackData);
            if (command != null)
            {
                await command.ExecuteAsync(botClient, null, callbackQuery, null, cancellationToken);
            }
        }

        async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            logger.LogInformation("HandleError: {ErrorMessage}", errorMessage);

            // Cooldown in case of network connection error
            if (exception is RequestException)
                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
        }
    }

    [HttpPost]
    [Route("Broadcast")]
    public async Task<IActionResult> BroadcastMessage(MessageRequest request)
    {
        request.ChatIds = new List<long> { -4034990842, 1142101264 };

        var messageText = request.Message;

        foreach (var userId in request.ChatIds)
        {
            try
            {
                await botClient.SendTextMessageAsync(chatId: userId, text: messageText);
            }
            catch (Exception ex)
            {
                // Handle errors here, e.g. user has blocked the bot, etc.
                Console.WriteLine($"Error sending message to user {userId}: {ex.Message}");
            }
        }

        return Ok(request.ChatIds);
    }
}