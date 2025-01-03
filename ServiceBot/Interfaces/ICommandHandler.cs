namespace W88.TeleBot.ServiceBot.Interfaces;

public interface ICommandHandler
{
    IBotCommand? GetBotCommand(string? commandText = null, string? commandName = null);
}