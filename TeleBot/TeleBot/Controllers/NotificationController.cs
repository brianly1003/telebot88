using Microsoft.AspNetCore.Mvc;
using TeleBot.ServiceBot;
using TeleBot.ServiceBot.Interfaces;
using TeleBot.ServiceBot.Models;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TeleBot.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IUserStateManager _userStateManager;
        private readonly ICommandFactory _commandFactory;

        public NotificationController(IUserStateManager userStateManager, ICommandFactory commandFactory,
            ITelegramBotClient botClient)
        {
            _userStateManager = userStateManager;
            _commandFactory = commandFactory;
            _botClient = botClient;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            using CancellationTokenSource cts = new();

            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };

            var me = await _botClient.GetMeAsync(cancellationToken: cts.Token);

            _botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );

            // Send cancellation request to stop bot
            cts.Cancel();

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

                if (_userStateManager.TryGetUserState(chatId, out var currentCommand))
                {
                    if (currentCommand != null)
                    {
                        await currentCommand.HandleResponse(botClient, message, cancellationToken);
                        return;
                    }
                }

                var command = _commandFactory.CreateCommand(me.Username, message);
                if (command != null)
                {
                    await command.ExecuteAsync(botClient, message, null, cancellationToken);
                }
            }

            async Task HandleCallbackQueryUpdate(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
            {
                var callbackData = callbackQuery.Data;
                var command = _commandFactory.CreateCommand(me.Username, messageText: callbackData);
                if (command != null)
                {
                    await command.ExecuteAsync(botClient, null, callbackQuery, cancellationToken);
                }
            }

            Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
            {
                var errorMessage = exception switch
                {
                    ApiRequestException apiRequestException
                        => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                    _ => exception.ToString()
                };

                Console.WriteLine(errorMessage);
                return Task.CompletedTask;
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
                    await _botClient.SendTextMessageAsync(chatId: userId, text: messageText);
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
}
