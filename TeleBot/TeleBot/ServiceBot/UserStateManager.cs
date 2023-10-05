using TeleBot.ServiceBot.Commands;
using Telegram.Bot.Types;
using Telegram.Bot;
using TeleBot.ServiceBot.Interfaces;

namespace TeleBot.ServiceBot
{
    public interface IUserStateManager
    {
        void SetUserState(long chatId, IBotCommand botCommand);
        bool TryGetUserState(long chatId, out IBotCommand? currentState);
        void RemoveUserState(long chatId);
    }

    public class UserStateManager : IUserStateManager
    {
        private readonly Dictionary<long, IBotCommand> _userStates = new();

        public void SetUserState(long chatId, IBotCommand command)
        {
            _userStates[chatId] = command;
        }

        public bool TryGetUserState(long chatId, out IBotCommand? command)
        {
            return _userStates.TryGetValue(chatId, out command);
        }

        public void RemoveUserState(long chatId)
        {
            _userStates.Remove(chatId);
        }

        public async Task HandleUserResponse(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            var chatId = message.Chat.Id;
            if (TryGetUserState(chatId, out var currentState))
            {
                if (currentState is ShareContactCommand shareContactCommand)
                {
                    await shareContactCommand.HandleResponse(botClient, message, cancellationToken);
                }
            }
        }
    }
}
