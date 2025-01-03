using CW88.TeleBot.ServiceBot;
using W88.TeleBot.ServiceBot.Constants;
using W88.TeleBot.ServiceBot.Interfaces;

namespace W88.TeleBot.ServiceBot.Commands.Deposit;

public class PaymentGatewayCommand(ICommandHandler commandHandler, IUserStateManager userStateManager)
    : BaseCommand(commandHandler, userStateManager)
{
    public override string Name => CommandNames.PaymentGatewayCommand;
    public override string CommandText => TextCommands.PaymentGateway;

    public override async Task ExecuteAsync(ITelegramBotClient botClient, Message? message, CallbackQuery? callback,
        InlineQuery? inlineQuery, CancellationToken cancellationToken)
    {
            
        }
}