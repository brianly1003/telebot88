//using Microsoft.AspNetCore.SignalR.Client;
//using Microsoft.Extensions.Options;
//using Telegram.Bot;
//using Telegram.Bot.Types;
//using W88.Shared.Dtos.ChatHub;
//using W88.Shared.Enums;
//using W88.Shared.Interfaces.ChatHub;
//using W88.Shared.Utils;
//using W88.TeleBot.Model;
//using W88.TeleBot.Model.Conversation;
//using W88.TeleBot.ServiceBot.Constants;
//using W88.TeleBot.ServiceBot.Interfaces;
//using W88.TeleBot.Services.Interfaces;
//using Message = Telegram.Bot.Types.Message;
//using User = W88.LiveChat.Proto.User;


//namespace W88.TeleBot.ServiceBot.Commands.Chat
//{
//    public class ChatCommand(
//        ICommandHandler commandHandler,
//        IUserStateManager userStateManager,
//        IOptions<CoreConfig> botConfig,
//        IUserService userService,
//        IConversationService conversationService)
//        : BaseCommand(commandHandler, userStateManager)
//    {
//        private readonly CoreConfig _botConfig = botConfig.Value;

//        private HubConnection? _connection;

//        private User? _currentUser;

//        private string _conversationId;
//        private string _threadTopic;
//        private readonly IUserStateManager _userStateManager = userStateManager;

//        public override string Name => CommandNames.ChatCommand;
//        public override string[] Commands { get; } = [TextCommands.Chat, TextCommands.ChatMode];

//        public override async Task ExecuteAsync(ITelegramBotClient botClient, Message? message,
//            CallbackQuery? callbackQuery, InlineQuery? inlineQuery, CancellationToken cancellationToken)
//        {
//            var chatId = message?.Chat.Id;
//            if (chatId == null) return;

//            try
//            {
//                var currentUser = await userService.GetUserByChatId((long)chatId);
//                if (currentUser == null)
//                {
//                    await botClient.SendTextMessageAsync(chatId, "User hasn't been registered!",
//                        cancellationToken: cancellationToken);
//                    return;
//                }

//                // Check if user is already in chat mode
//                var userState = await _userStateManager.TryGetUserState((long)chatId);
//                if (userState is { IsSuccess: true })
//                {
//                    await botClient.SendTextMessageAsync(chatId, "You are already in chat mode!",
//                        cancellationToken: cancellationToken);
//                    return;
//                }

//                // Create conversation if not exist:
//                // Input botID => Find user are attached with?
//                var botUser = await userService.GetUserByChatId((long)botClient.BotId!);
//                if (botUser == null)
//                {
//                    await botClient.SendTextMessageAsync(chatId, "Cannot find Bot",
//                        cancellationToken: cancellationToken);
//                    return;
//                }

//                var botConversation = await conversationService.GetConversation(new GetConversationArgs
//                {
//                    SenderId = currentUser.UserId,
//                    ReceiverIds = [botUser.UserId],
//                    IsGroup = false
//                }) ?? await conversationService.CreateConversation(new CreateConversationArgs
//                {
//                    SenderId = currentUser.UserId,
//                    ReceiverIds = [botUser.UserId],
//                    IsGroup = false
//                });

//                _conversationId = botConversation.ConversationId;
//                _threadTopic = botConversation.ThreadTopic;
//                _currentUser = currentUser;
//                _connection = new HubConnectionBuilder()
//                    .WithUrl(
//                        $"{_botConfig.IGChatServiceUrl}/chathub?channel={(int)ChannelType.Telegram}&userId={currentUser.UserId}")
//                    .WithAutomaticReconnect()
//                    .Build();

//                _connection.On<MessageDto>(nameof(IChatHubClient.ReceiveMessage), async (receiveMessage) =>
//                {
//                    var (senderId, content) = receiveMessage;

//                    if (senderId != _currentUser.UserId && !string.IsNullOrWhiteSpace(content))
//                    {
//                        await botClient.SendTextMessageAsync(chatId, content,
//                            cancellationToken: CancellationToken.None);
//                    }
//                });

//                //connection.Closed += async (error) =>
//                //{
//                //    await Task.Delay(new Random().Next(0, 5) * 1000, cancellationToken);
//                //    await connection.StartAsync(cancellationToken);
//                //};

//                await _connection.StartAsync(cancellationToken);

//                if (_connection.State == HubConnectionState.Connected)
//                {
//                    // Set the user state
//                    await _userStateManager.SetUserState((long)chatId, this);

//                    await botClient.SendTextMessageAsync(chatId,
//                        $"Start chat mode with the support agent. Type \"/exit\" to end the chat mode.",
//                        cancellationToken: cancellationToken);
//                }
//                else
//                {
//                    await botClient.SendTextMessageAsync(chatId, $"Cannot stat using chat mode",
//                        cancellationToken: cancellationToken);
//                }
//            }
//            catch (Exception ex)
//            {
//                await botClient.SendTextMessageAsync(chatId, ex.Message, cancellationToken: cancellationToken);
//            }
//        }

//        public override async Task HandleResponse(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
//        {
//            var chatId = message.Chat.Id;
//            var messageText = message.Text?.Trim();

//            if (messageText == TextCommands.ExitChatMode || TextCommands.AllCommands.Contains(messageText))
//            {
//                // Handle exit chat mode
//                await _userStateManager.RemoveUserState(chatId);
//                await _connection.StopAsync(cancellationToken);
//                await botClient.SendTextMessageAsync(chatId, "Exit success!", cancellationToken: CancellationToken.None);

//                // Execute command if the message is a valid command
//                if (TextCommands.AllCommands.Contains(messageText))
//                {
//                    var command = commandHandler.GetBotCommand(commandText: messageText);
//                    if (command != null)
//                    {
//                        await command.ExecuteAsync(botClient, message, null, null, cancellationToken);
//                    }
//                }
//            }
//            else if (_connection.State == HubConnectionState.Connected)
//            {
//                var chatMessage = string.Empty;

//                // Check if the message contains a sticker emoji
//                if (string.IsNullOrEmpty(message.Text) && !string.IsNullOrEmpty(message.Sticker?.Emoji))
//                {
//                    chatMessage = message.Sticker.Emoji;
//                }
//                else if (!string.IsNullOrEmpty(message.Text))
//                {
//                    chatMessage = message.Text;
//                }

//                // Dispatch message if it's not empty or whitespace
//                if (!string.IsNullOrWhiteSpace(chatMessage))
//                {
//                    var newMessage = new MessageDto
//                    {
//                        Id = new SnowflakeId().NextId().ToString(),
//                        SenderId = _currentUser.UserId,
//                        Content = chatMessage,
//                        FullName = _currentUser.FullName,
//                        ConversationId = _conversationId,
//                        ThreadTopic = _threadTopic,
//                        Channel = (int)ChannelType.Telegram,
//                        CreatedTimeStamp = DateTime.UtcNow.ToUnixTimestamp()
//                    };

//                    await _connection.InvokeAsync(nameof(IChatHubServer.DispatchMessage), newMessage, cancellationToken: cancellationToken);
//                }
//            }
//        }
//    }
//}