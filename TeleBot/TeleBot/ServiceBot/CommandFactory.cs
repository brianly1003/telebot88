using TeleBot.ServiceBot.Constants;
using TeleBot.ServiceBot.Interfaces;
using Telegram.Bot.Types;

namespace TeleBot.ServiceBot
{
    public class CommandFactory : ICommandFactory
    {
        private readonly IUserStateManager _userStateManager;
        private readonly ICommandHandler _commandHandler;

        public CommandFactory(IUserStateManager userStateManager, ICommandHandler commandHandler)
        {
            _userStateManager = userStateManager;
            _commandHandler = commandHandler;
        }

        public IBotCommand? CreateCommand(string? botName, Message? message = null, string? messageText = null)
        {
            var commandText = GetCommandText(botName, message, messageText);

            //if (string.IsNullOrWhiteSpace(commandText) || !TextCommands.AllCommands.Contains(commandText))
            //    return null;

            return string.IsNullOrWhiteSpace(commandText) ? null : _commandHandler.GetBotCommand(commandText);
        }

        private string GetCommandText(string? botName, Message? message, string? messageText)
        {
            if (message?.Contact != null)
                return TextCommands.ShareContact;

            var commandText = message?.Text ?? messageText ?? string.Empty;

            if (botName != null && commandText.StartsWith($"@{botName} "))
                commandText = commandText[(botName.Length + 2)..].Trim();

            return commandText;
        }
    }
}
