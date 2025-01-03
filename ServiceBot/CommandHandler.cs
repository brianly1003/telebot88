using W88.TeleBot.ServiceBot.Interfaces;
using W88.TeleBot.Services;

namespace W88.TeleBot.ServiceBot;

public class CommandHandler(ServiceLocator serviceLocator) : ICommandHandler
{
    public IBotCommand? GetBotCommand(string? commandText, string? commandName)
    {
        var commands = serviceLocator.GetServices<IBotCommand>();
        IBotCommand? command = null;

        if (!string.IsNullOrEmpty(commandName))
        {
            command = commands.FirstOrDefault(c => c.Name == commandName);
        }
        else if (!string.IsNullOrEmpty(commandText))
        {
            command = commands.FirstOrDefault(c =>
                c.CommandText == commandText || c.Commands.Contains(commandText));
        }

        return command;
    }
}