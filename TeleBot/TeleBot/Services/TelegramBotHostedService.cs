using Microsoft.Extensions.Options;
using TeleBot.Models;
using TeleBot.ServiceBot;
using TeleBot.ServiceBot.Constants;
using TeleBot.ServiceBot.Interfaces;
using TeleBot.ServiceBot.Utils;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TeleBot.Services
{
    public class TelegramBotHostedService : IHostedService
    {
        private readonly ILogger<TelegramBotHostedService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly BotConfiguration _botConfig;

        private readonly ITelegramBotClient _botClient;
        private readonly IUserStateManager _userStateManager;
        private readonly ICommandFactory _commandFactory;

        private readonly Dictionary<long, bool> _hasLoadedForUser = new();

        public TelegramBotHostedService(ITelegramBotClient botClient, IUserStateManager userStateManager,
            ICommandFactory commandFactory, ILogger<TelegramBotHostedService> logger,
            IServiceProvider serviceProvider,
            IOptions<BotConfiguration> botOptions)
        {
            _botClient = botClient;
            _userStateManager = userStateManager;
            _commandFactory = commandFactory;

            _logger = logger;
            _serviceProvider = serviceProvider;
            _botConfig = botOptions.Value;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

            //var webhookAddress = $"{_botConfig.HostAddress}{_botConfig.Route}";
            //_logger.LogInformation("Setting webhook: {WebhookAddress}", webhookAddress);
            //await botClient.SetWebhookAsync(
            //    url: webhookAddress,
            //    allowedUpdates: Array.Empty<UpdateType>(),
            //    secretToken: _botConfig.SecretToken,
            //    cancellationToken: cancellationToken);

            await botClient.DeleteWebhookAsync(cancellationToken: cancellationToken);

            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };

            var me = await botClient.GetMeAsync(cancellationToken: cancellationToken);

            botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cancellationToken
            );

            async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
            {
                try
                {
                    var chatId = update.Message?.Chat.Id ?? update.CallbackQuery?.Message?.Chat.Id;
                    if (chatId == null) return;

                    await InitialLoading(botClient, update.Message, cancellationToken);

                    switch (update.Type)
                    {
                        // A message was received
                        case UpdateType.Message:
                            await HandleMessageUpdate(botClient, update.Message!, cancellationToken);
                            break;

                        // A button was pressed
                        case UpdateType.CallbackQuery:
                            await HandleCallbackQueryUpdate(botClient, update.CallbackQuery!, cancellationToken);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    await HandlePollingErrorAsync(botClient, ex, cancellationToken);
                }
            }

            async Task HandleMessageUpdate(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
            {
                var chatId = message.Chat.Id;

                if (_userStateManager.TryGetUserState(chatId, out var currentCommand))
                {
                    if (currentCommand != null)
                    {
                        await currentCommand.UserInteractive(botClient, message, cancellationToken);
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
                var callbackDataString = callbackQuery.Data ?? string.Empty;
                var callbackDataObject = DataExtensions.BuildCallbackData(callbackDataString);

                var commandText = callbackDataObject == null ? callbackDataString : callbackDataObject.CommandName;
                var command = _commandFactory.CreateCommand(me.Username, messageText: commandText);
                if (command != null)
                {
                    await command.ExecuteAsync(botClient, callbackQuery.Message, callbackQuery, cancellationToken);
                }
            }

            async Task InitialLoading(ITelegramBotClient botClient, Message? message, CancellationToken cancellationToken)
            {
                if (message == null || message.Chat.Type != ChatType.Private)
                    return;

                var chatId = message.Chat.Id;

                if (!_hasLoadedForUser.TryGetValue(chatId, out var hasLoaded) || !hasLoaded)
                {
                    await botClient.SetChatMenuButtonAsync(chatId: chatId, menuButton: new MenuButtonCommands(), cancellationToken: cancellationToken);
                    await botClient.SetMyCommandsAsync(new List<BotCommand>
                    {
                        new() { Command = TextCommands.Start, Description = "Start" },
                        new() { Command = TextCommands.Play, Description = "Play" }
                    }, cancellationToken: cancellationToken);

                    _hasLoadedForUser[chatId] = true;
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

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

            await botClient.DeleteWebhookAsync(cancellationToken: cancellationToken);
        }
    }
}
