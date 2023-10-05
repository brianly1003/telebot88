using TeleBot.ServiceBot.Constants;
using TeleBot.ServiceBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TeleBot.ServiceBot
{
    public abstract class BaseCommand : IBotCommand
    {
        private readonly IUserStateManager _userStateManager;
        private readonly ICommandHandler _commandHandler;
        public abstract string Name { get; }
        public abstract string CommandText { get; }

        protected BaseCommand(ICommandHandler commandHandler, IUserStateManager userStateManager)
        {
            _commandHandler = commandHandler;
            _userStateManager = userStateManager;
        }

        public abstract Task ExecuteAsync(ITelegramBotClient botClient, Message? message, CallbackQuery? callbackQuery, CancellationToken cancellationToken);

        public virtual Task HandleResponse(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public virtual async Task UserInteractive(ITelegramBotClient botClient, Message? message, CancellationToken cancellationToken)
        {
            var chatId = message.Chat.Id;
            var messageText = message.Text?.Trim().ToLower() ?? string.Empty;

            if (TextCommands.AllCommands.Contains(messageText))
            {
                _userStateManager.RemoveUserState(chatId);
                var nextCommand = _commandHandler.GetBotCommand(messageText);
                if (nextCommand != null)
                {
                    await nextCommand.ExecuteAsync(botClient, message, null, cancellationToken);
                }
            }
            else
            {
                await HandleResponse(botClient, message, cancellationToken);
            }
        }
    }
}
