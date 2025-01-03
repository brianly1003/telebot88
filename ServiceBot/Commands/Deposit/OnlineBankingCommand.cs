using CW88.TeleBot.ServiceBot;
using Telegram.Bot.Types.ReplyMarkups;
using W88.TeleBot.ServiceBot.Constants;
using W88.TeleBot.ServiceBot.Interfaces;
using W88.TeleBot.ServiceBot.Models;
using W88.TeleBot.ServiceBot.Utils;

namespace W88.TeleBot.ServiceBot.Commands.Deposit;

public class OnlineBankingCommand(ICommandHandler commandHandler, IUserStateManager userStateManager)
    : BaseCommand(commandHandler, userStateManager)
{
    public override string Name => CommandNames.OnlineBankingCommand;

    public override string CommandText => TextCommands.OnlineBanking;

    public override async Task ExecuteAsync(ITelegramBotClient botClient, Message? message, CallbackQuery? callback,
        InlineQuery? inlineQuery, CancellationToken cancellationToken)
    {
        try
        {
            var chatId = message?.Chat.Id ?? -1;
            var messageId = message?.MessageId ?? -1;

            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(TextCommands.UnionBank),
                    InlineKeyboardButton.WithCallbackData(TextCommands.BPIBank)
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(TextCommands.MetroBank),
                    InlineKeyboardButton.WithCallbackData(TextCommands.Empty)
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(TextCommands.Back,
                        DataExtensions.SerializeCallbackData(new CallbackData(TextCommands.Back,
                            CommandNames.DepositCommand)))
                }
            });

            var textMsg =
                "Please select" +
                $"\n\nUpdated On: {DateTime.Now:d/MM/yyyy hh:mm:ss tt}";

            _ = await botClient.EditMessageText(
                chatId: chatId,
                messageId: messageId,
                text: textMsg,
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}