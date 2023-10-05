using TeleBot.ServiceBot.Constants;
using TeleBot.ServiceBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleBot.ServiceBot.Commands
{
    public class MainCommand : BaseCommand
    {
        public override string Name => CommandNames.MainCommand;
        public override string CommandText => string.Empty;

        public MainCommand(ICommandHandler commandHandler, IUserStateManager userStateManager) : base(commandHandler, userStateManager)
        {
        }

        public override async Task ExecuteAsync(ITelegramBotClient botClient, Message? message, CallbackQuery? callback, CancellationToken cancellationToken)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
                new KeyboardButton[] { TextCommands.Games },
                new KeyboardButton[] { TextCommands.Deposit, TextCommands.Transfer, TextCommands.Withdraw },
                new KeyboardButton[] { TextCommands.Balance, TextCommands.Profile, TextCommands.Bank },
                new KeyboardButton[] { TextCommands.Promotion, TextCommands.History, TextCommands.AboutUs },
                new KeyboardButton[] { TextCommands.ContactUs, TextCommands.Setting }
            })
            {
                ResizeKeyboard = true
            };

            var chatId = message?.Chat.Id ?? -1;

            var _ = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Please select",
                replyMarkup: replyKeyboardMarkup,
                cancellationToken: cancellationToken);
        }
    }
}
