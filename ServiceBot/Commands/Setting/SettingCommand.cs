using W88.TeleBot.ServiceBot.Constants;
using W88.TeleBot.ServiceBot.Interfaces;

namespace W88.TeleBot.ServiceBot.Commands.Setting;

public class SettingCommand(ICommandHandler commandHandler, IUserStateManager userStateManager)
    : BaseCommand(commandHandler, userStateManager)
{
    public override string Name => CommandNames.SettingCommand;
    public override string CommandText => TextCommands.Setting;

    public override async Task ExecuteAsync(ITelegramBotClient botClient, Message? message, CallbackQuery? callback,
        InlineQuery? inlineQuery, CancellationToken cancellationToken)
    {
            await base.ExecuteAsync(botClient, message, callback, inlineQuery, cancellationToken);
        }
}