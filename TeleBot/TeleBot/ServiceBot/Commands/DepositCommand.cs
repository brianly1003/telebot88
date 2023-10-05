using Newtonsoft.Json;
using TeleBot.ServiceBot.Constants;
using TeleBot.ServiceBot.Interfaces;
using TeleBot.ServiceBot.Models;
using TeleBot.ServiceBot.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleBot.ServiceBot.Commands
{
    public class DepositCommand : BaseCommand
    {
        public override string Name => CommandNames.DepositCommand;

        public override string CommandText => TextCommands.Deposit;

        public DepositCommand(ICommandHandler commandHandler, IUserStateManager userStateManager) : base(commandHandler, userStateManager)
        {
        }

        public override async Task ExecuteAsync(ITelegramBotClient botClient, Message? message, CallbackQuery? callbackQuery, CancellationToken cancellationToken)
        {
            var chatId = message?.Chat.Id;
            if (chatId == null) return;

            var messageId = message?.MessageId ?? -1;

            if (callbackQuery == null)
            {
                var callbackDataObject = new CallbackData { CommandText = TextCommands.Cancel, CommandName = CommandNames.DepositCommand };
                var callbackDataString = JsonConvert.SerializeObject(callbackDataObject);

                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    new [] { InlineKeyboardButton.WithCallbackData(TextCommands.OnlineBanking), InlineKeyboardButton.WithCallbackData(TextCommands.PaymentGateway) },
                    new [] { InlineKeyboardButton.WithCallbackData(TextCommands.Cancel, callbackDataString) }
                });

                var textMsg =
                    "Please select" +
                    $"\n\nUpdated On: {DateTime.Now:d/MM/yyyy hh:mm:ss tt}";

                var _ = await botClient.SendTextMessageAsync(
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

                        var _ = await botClient.EditMessageTextAsync(
                            chatId: chatId,
                            messageId: messageId,
                            text: textMsg,
                            replyMarkup: null,
                            cancellationToken: cancellationToken);
                        break;
                    }
                    case TextCommands.Back:
                    {
                        var callbackDataObject = new CallbackData { CommandText = TextCommands.Cancel, CommandName = CommandNames.DepositCommand };
                        var callbackDataString = JsonConvert.SerializeObject(callbackDataObject);

                        var inlineKeyboard = new InlineKeyboardMarkup(new[]
                        {
                            new [] { InlineKeyboardButton.WithCallbackData(TextCommands.OnlineBanking), InlineKeyboardButton.WithCallbackData(TextCommands.PaymentGateway) },
                            new [] { InlineKeyboardButton.WithCallbackData(TextCommands.Cancel, callbackDataString) }
                        });

                        var textMsg =
                            "Please select" +
                            $"\n\nUpdated On: {DateTime.Now:d/MM/yyyy hh:mm:ss tt}";

                        var _ = await botClient.EditMessageTextAsync(
                            chatId: chatId,
                            messageId: messageId,
                            text: textMsg,
                            replyMarkup: inlineKeyboard,
                            cancellationToken: cancellationToken);
                        break;
                    }
                }
            }
        }
    }
}
