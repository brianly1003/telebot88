using TeleBot.ServiceBot.Constants;
using TeleBot.ServiceBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleBot.ServiceBot.Commands
{
    public class GameCommand : BaseCommand
    {
        public override string Name => CommandNames.MainCommand;

        public override string CommandText => TextCommands.Games;

        public GameCommand(ICommandHandler commandHandler, IUserStateManager userStateManager) : base(commandHandler, userStateManager)
        {
        }

        public override async Task ExecuteAsync(ITelegramBotClient botClient, Message? message, CallbackQuery? callback, CancellationToken cancellationToken)
        {
            var chatId = message?.Chat.Id;
            if (chatId == null) return;

            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new [] { InlineKeyboardButton.WithCallbackData(TextCommands.Sport), InlineKeyboardButton.WithCallbackData(TextCommands.Casino), InlineKeyboardButton.WithCallbackData(TextCommands.Slot) },
                new [] { InlineKeyboardButton.WithCallbackData(TextCommands.Fishing), InlineKeyboardButton.WithCallbackData(TextCommands.Lottery), InlineKeyboardButton.WithCallbackData(TextCommands.ESport) },
                new [] { InlineKeyboardButton.WithCallbackData(TextCommands.CardnBoard), InlineKeyboardButton.WithCallbackData(TextCommands.CockFighting) },
                new [] { InlineKeyboardButton.WithCallbackData(TextCommands.Cancel, CommandNames.StartCommand) }
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
    }
}
