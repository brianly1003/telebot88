using CW88.TeleBot.ServiceBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CW88.TeleBot.ServiceBot.Commands.Games;

public class GameDetailCommand : BaseCommand
{
    public override string Name => string.Empty;
    public override string CommandText => string.Empty;

    public GameDetailCommand(ICommandHandler commandHandler, IUserStateManager userStateManager) : base(commandHandler, userStateManager)
    {
    }

    public override async Task ExecuteAsync(ITelegramBotClient botClient, Message? message, CallbackQuery? callback, InlineQuery? inlineQuery, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}