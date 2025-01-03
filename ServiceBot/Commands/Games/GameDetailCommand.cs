using W88.TeleBot.ServiceBot.Interfaces;

namespace W88.TeleBot.ServiceBot.Commands.Games;

public class GameDetailCommand(ICommandHandler commandHandler, IUserStateManager userStateManager)
    : BaseCommand(commandHandler, userStateManager)
{
    public override string Name => string.Empty;
    public override string CommandText => string.Empty;

    public override async Task ExecuteAsync(ITelegramBotClient botClient, Message? message, CallbackQuery? callback, InlineQuery? inlineQuery, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}