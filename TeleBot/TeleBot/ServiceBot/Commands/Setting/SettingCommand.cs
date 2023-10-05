using TeleBot.ServiceBot.Constants;
using TeleBot.ServiceBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TeleBot.ServiceBot.Commands.Setting
{
    public class SettingCommand : BaseCommand
    {
        public override string Name => CommandNames.SettingCommand;
        public override string CommandText => TextCommands.Setting;

        public SettingCommand(ICommandHandler commandHandler, IUserStateManager userStateManager) : base(commandHandler, userStateManager)
        {
        }

        public override Task ExecuteAsync(ITelegramBotClient botClient, Message? message, CallbackQuery? callbackQuery,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
