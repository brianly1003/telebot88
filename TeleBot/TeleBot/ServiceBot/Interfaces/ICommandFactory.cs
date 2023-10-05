using Telegram.Bot.Types;

namespace TeleBot.ServiceBot.Interfaces
{
    public interface ICommandFactory
    {
        IBotCommand? CreateCommand(string? botName, Message? message = null, string? messageText = null);
    }

}
