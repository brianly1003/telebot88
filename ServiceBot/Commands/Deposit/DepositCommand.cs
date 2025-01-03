using Newtonsoft.Json;
using Telegram.Bot.Types.ReplyMarkups;
using W88.TeleBot.ServiceBot.Constants;
using W88.TeleBot.ServiceBot.Interfaces;
using W88.TeleBot.ServiceBot.Models;
using W88.TeleBot.ServiceBot.Utils;

namespace W88.TeleBot.ServiceBot.Commands.Deposit;

public class DepositCommand(ICommandHandler commandHandler, IUserStateManager userStateManager)
    : BaseCommand(commandHandler, userStateManager)
{
    public override string Name => CommandNames.DepositCommand;

    public override string CommandText => TextCommands.Deposit;

    public override async Task ExecuteAsync(ITelegramBotClient botClient, Message? message,
        CallbackQuery? callbackQuery, InlineQuery? inlineQuery, CancellationToken cancellationToken)
    {
        var chatId = message?.Chat.Id;
        if (chatId == null) return;

        var messageId = message?.MessageId ?? -1;

        if (callbackQuery == null)
        {
            var callbackDataObject = new CallbackData(TextCommands.Cancel, CommandNames.DepositCommand);
            var callbackDataString = JsonConvert.SerializeObject(callbackDataObject);

            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                [
                    InlineKeyboardButton.WithCallbackData(TextCommands.OnlineBanking),
                    InlineKeyboardButton.WithCallbackData(TextCommands.PaymentGateway)
                ],
                new[] { InlineKeyboardButton.WithCallbackData(TextCommands.Cancel, callbackDataString) }
            });

            var textMsg =
                "Please select" +
                $"\n\nUpdated On: {DateTime.Now:d/MM/yyyy hh:mm:ss tt}";

            _ = await botClient.SendMessage(
                chatId: chatId,
                text: textMsg,
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken);
        }
        else
        {
            var callbackData = DataExtensions.BuildCallbackData(callbackQuery?.Data ?? string.Empty);

            // Close the query to end the client-side loading animation
            //await botClient.AnswerCallbackQueryAsync(callbackQueryId: callbackQuery?.Id!, text: "TEST...", cancellationToken: cancellationToken);

            switch (callbackData?.CommandText)
            {
                case TextCommands.Cancel:
                {
                    const string textMsg = "Action cancelled.";

                    _ = await botClient.EditMessageText(
                        chatId: chatId,
                        messageId: messageId,
                        text: textMsg,
                        replyMarkup: null,
                        cancellationToken: cancellationToken);
                    break;
                }
                case TextCommands.Back:
                {
                    var callbackDataObject = new CallbackData(TextCommands.Cancel, CommandNames.DepositCommand);
                    var callbackDataString = JsonConvert.SerializeObject(callbackDataObject);

                    var inlineKeyboard = new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData(TextCommands.OnlineBanking),
                            InlineKeyboardButton.WithCallbackData(TextCommands.PaymentGateway)
                        },
                        new[] { InlineKeyboardButton.WithCallbackData(TextCommands.Cancel, callbackDataString) }
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
                    break;
                }
                default:
                    if (callbackQuery?.Data == CommandText)
                    {
                        var callbackDataObject = new CallbackData(TextCommands.Cancel, CommandNames.DepositCommand);
                        var callbackDataString = JsonConvert.SerializeObject(callbackDataObject);

                        var inlineKeyboard = new InlineKeyboardMarkup(new[]
                        {
                            new[]
                            {
                                InlineKeyboardButton.WithCallbackData(TextCommands.OnlineBanking),
                                InlineKeyboardButton.WithCallbackData(TextCommands.PaymentGateway)
                            },
                            new[] { InlineKeyboardButton.WithCallbackData(TextCommands.Cancel, callbackDataString) }
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

                    break;
            }
        }
    }
}