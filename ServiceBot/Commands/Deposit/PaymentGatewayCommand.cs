using CW88.TeleBot.ServiceBot.Constants;
using CW88.TeleBot.ServiceBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CW88.TeleBot.ServiceBot.Commands.Deposit;

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