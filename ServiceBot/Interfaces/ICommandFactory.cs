namespace W88.TeleBot.ServiceBot.Interfaces;

public interface ICommandFactory
{
    IBotCommand? CreateCommand(string? botName, Message? message = null, InlineQuery? inlineQuery = null, string? commandText = null, string? commandName = null);
}