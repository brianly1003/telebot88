using CW88.TeleBot.ServiceBot.Constants;
using CW88.TeleBot.ServiceBot.Interfaces;
using CW88.TeleBot.ServiceBot.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CW88.TeleBot.ServiceBot.Commands.Bank;

public class BankCommand(ICommandHandler commandHandler, IUserStateManager userStateManager)
    : BaseCommand(commandHandler, userStateManager)
{
    public override string Name => CommandNames.BankCommand;
    public override string CommandText => TextCommands.Bank;

    public override async Task ExecuteAsync(ITelegramBotClient botClient, Message? message,
        CallbackQuery? callbackQuery, InlineQuery? inlineQuery, CancellationToken cancellationToken)
    {
            var chatId = message?.Chat.Id;
            if (chatId == null) return;

            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[] { InlineKeyboardButton.WithCallbackData(TextCommands.AddBankAccount) },
                new[] { InlineKeyboardButton.WithCallbackData(TextCommands.Cancel) }
            });

            var textMsg =
                "Please add bank account" +
                $"\n\nUpdated On: {DateTime.Now:d/MM/yyyy hh:mm:ss tt}";

            if (callbackQuery == null)
            {
                _ = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: textMsg,
                    replyMarkup: inlineKeyboard,
                    cancellationToken: cancellationToken);
            }
            else
            {
                var callbackData = DataExtensions.BuildCallbackData(callbackQuery.Data ?? string.Empty);
                if (callbackData?.CommandText == TextCommands.Back)
                {
                    var messageId = message?.MessageId ?? -1;

                    _ = await botClient.EditMessageTextAsync(
                        chatId: chatId,
                        messageId: messageId,
                        text: textMsg,
                        replyMarkup: inlineKeyboard,
                        cancellationToken: cancellationToken);
                }
            }
        }
}