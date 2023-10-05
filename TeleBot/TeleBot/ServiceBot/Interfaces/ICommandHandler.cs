namespace TeleBot.ServiceBot.Interfaces
{
    public interface ICommandHandler
    {
        IBotCommand? GetBotCommand(string commandText);
    }
}
