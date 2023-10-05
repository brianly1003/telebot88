using Telegram.Bot.Types;
using Telegram.Bot;
using TeleBot.ServiceBot.Interfaces;
using TeleBot.Services;

namespace TeleBot.ServiceBot
{
    public class CommandHandler : ICommandHandler
    {
        private readonly IDictionary<string, IBotCommand> _commands = new Dictionary<string, IBotCommand>();
        private readonly ServiceLocator _serviceLocator;

        public CommandHandler(ServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        public IBotCommand? GetBotCommand(string commandText)
        {
            //if (_commands.TryGetValue(commandText, out var command)) return command;

            var commands = _serviceLocator.GetServices<IBotCommand>();
            var command = commands.FirstOrDefault(c => c.CommandText == commandText || c.Name == commandText);

            if (command != null) _commands[command.Name] = command;
            return command;
        }
    }
}
