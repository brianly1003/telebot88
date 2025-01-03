using Telegram.Bot.Types.ReplyMarkups;
using W88.TeleBot.ServiceBot.Constants;
using W88.TeleBot.ServiceBot.Interfaces;
using W88.TeleBot.ServiceBot.Models;
using W88.TeleBot.ServiceBot.Utils;
using W88.TeleBot.Services.Interfaces;

namespace W88.TeleBot.ServiceBot.Commands.Bank;

public class AddBankAccountCommand(
    ICommandHandler commandHandler,
    IUserStateManager userStateManager,
    IBankService bankService)
    : BaseCommand(commandHandler, userStateManager)
{
    public override string Name => CommandNames.AddBankAccountCommand;
    public override string CommandText => TextCommands.AddBankAccount;

    public override async Task ExecuteAsync(ITelegramBotClient botClient, Message? message, CallbackQuery? callback,
        InlineQuery? inlineQuery, CancellationToken cancellationToken)
    {
        var chatId = message?.Chat.Id;
        if (chatId == null) return;

        var bankList = await bankService.GetBankInfos();
        var inlineKeyboard = GenerateInlineKeyboard(bankList, columns: 2);

        var textMsg =
            "Please select" +
            $"\n\nUpdated On: {DateTime.Now:d/MM/yyyy hh:mm:ss tt}";

        _ = await botClient.SendMessage(
            chatId: chatId,
            text: textMsg,
            replyMarkup: inlineKeyboard,
            cancellationToken: cancellationToken);
    }

    private static InlineKeyboardMarkup GenerateInlineKeyboard(ICollection<string> bankList, int columns)
    {
        var rows = new List<InlineKeyboardButton[]>();

        if (bankList.Count % columns != 0)
        {
            bankList.Add(""); // Add an empty bank item
        }

        for (var i = 0; i < bankList.Count; i += columns)
        {
            var chunk = bankList.Skip(i).Take(columns).ToList();
            var buttons = chunk.Select(bank =>
                string.IsNullOrWhiteSpace(bank)
                    ? InlineKeyboardButton.WithCallbackData(TextCommands.Empty)
                    : InlineKeyboardButton.WithCallbackData(bank)
            ).ToArray();
            rows.Add(buttons);
        }

        // Adding the Back command to the end
        rows.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData(TextCommands.Back,
                DataExtensions.SerializeCallbackData(new CallbackData(TextCommands.Back, CommandNames.BankCommand)))
        });

        return new InlineKeyboardMarkup(rows);
    }
}