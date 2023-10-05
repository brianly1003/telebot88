using Newtonsoft.Json;
using TeleBot.ServiceBot.Constants;
using TeleBot.ServiceBot.Interfaces;
using TeleBot.ServiceBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleBot.ServiceBot.Commands.Deposit
{
    public class OnlineBankingCommand : BaseCommand
    {
        public override string Name => CommandNames.OnlineBankingCommand;

        public override string CommandText => TextCommands.OnlineBanking;

        public OnlineBankingCommand(ICommandHandler commandHandler, IUserStateManager userStateManager) : base(commandHandler, userStateManager)
        {
        }

        public override async Task ExecuteAsync(ITelegramBotClient botClient, Message? message, CallbackQuery? callback, CancellationToken cancellationToken)
        {
            try
            {
                var chatId = message?.Chat.Id ?? -1;
                var messageId = message?.MessageId ?? -1;

                var callbackDataObject = new CallbackData { CommandText = TextCommands.Back, CommandName = CommandNames.DepositCommand };
                var callbackDataString = JsonConvert.SerializeObject(callbackDataObject);

                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    new [] { InlineKeyboardButton.WithCallbackData(TextCommands.UnionBank), InlineKeyboardButton.WithCallbackData(TextCommands.BPIBank) },
                    new [] { InlineKeyboardButton.WithCallbackData(TextCommands.MetroBank), InlineKeyboardButton.WithCallbackData(TextCommands.Empty) },
                    new [] { InlineKeyboardButton.WithCallbackData(TextCommands.Back, callbackDataString) }
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
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
