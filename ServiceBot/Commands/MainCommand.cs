using CW88.TeleBot.ServiceBot;
using Telegram.Bot.Types.ReplyMarkups;
using W88.TeleBot.ServiceBot.Constants;
using W88.TeleBot.ServiceBot.Interfaces;

namespace W88.TeleBot.ServiceBot.Commands;

public class MainCommand(ICommandHandler commandHandler, IUserStateManager userStateManager)
    : BaseCommand(commandHandler, userStateManager)
{
    public override string Name => CommandNames.MainCommand;
    public override string CommandText => string.Empty;

    public override async Task ExecuteAsync(ITelegramBotClient botClient, Message? message, CallbackQuery? callback, InlineQuery? inlineQuery, CancellationToken cancellationToken)
    {
            var random = new Random();

            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
                new KeyboardButton[] { new(TextCommands.ChatMode) {Text = TextCommands.Chat}, TextCommands.Games },
                new KeyboardButton[] { TextCommands.Deposit, TextCommands.Transfer, TextCommands.Withdraw },
                new KeyboardButton[] { new(TextCommands.Balance) { Text = $"{TextCommands.Balance} ({random.Next(0,100):##.00})"}, TextCommands.Profile, TextCommands.Bank },
                new KeyboardButton[] { TextCommands.Promotion, TextCommands.History, TextCommands.AboutUs },
                new KeyboardButton[] { TextCommands.ContactUs, TextCommands.Setting }
            })
            {
                ResizeKeyboard = true
            };

            var chatId = message?.Chat.Id ?? -1;

            _ = await botClient.SendMessage(
                chatId: chatId,
                text: "Please select",
                replyMarkup: replyKeyboardMarkup,
                cancellationToken: cancellationToken);
        }
}