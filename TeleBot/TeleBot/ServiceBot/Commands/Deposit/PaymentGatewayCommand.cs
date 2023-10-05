using TeleBot.ServiceBot.Constants;
using TeleBot.ServiceBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TeleBot.ServiceBot.Commands.Deposit
{
    public class PaymentGatewayCommand : BaseCommand
    {
        public override string Name => CommandNames.PaymentGatewayCommand;
        public override string CommandText => TextCommands.PaymentGateway;

        public PaymentGatewayCommand(ICommandHandler commandHandler, IUserStateManager userStateManager) : base(commandHandler, userStateManager)
        {
        }

        public override Task ExecuteAsync(ITelegramBotClient botClient, Message? message, CallbackQuery? callbackQuery, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
