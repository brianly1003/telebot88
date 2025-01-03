using CW88.TeleBot.ServiceBot.Constants;
using CW88.TeleBot.ServiceBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CW88.TeleBot.ServiceBot.Commands;

public class GameCommand(ICommandHandler commandHandler, IUserStateManager userStateManager)
    : BaseCommand(commandHandler,
        userStateManager)
{
    public override string Name => CommandNames.GameCommand;

    public override string CommandText => TextCommands.Games;

    public override async Task ExecuteAsync(ITelegramBotClient botClient, Message? message, CallbackQuery? callback,
        InlineQuery? inlineQuery, CancellationToken cancellationToken)
    {
            var chatId = message?.Chat.Id;
            if (chatId == null) return;

            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(TextCommands.Sport),
                    InlineKeyboardButton.WithCallbackData(TextCommands.Casino),
                    InlineKeyboardButton.WithCallbackData(TextCommands.Slot)
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(TextCommands.Fishing),
                    InlineKeyboardButton.WithCallbackData(TextCommands.Lottery),
                    InlineKeyboardButton.WithCallbackData(TextCommands.ESport)
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(TextCommands.CardnBoard),
                    InlineKeyboardButton.WithCallbackData(TextCommands.CockFighting)
                },
                new[] { InlineKeyboardButton.WithCallbackData(TextCommands.Cancel) }
            });

            var textMsg =
                "Please select" +
                $"\n\nUpdated On: {DateTime.Now:d/MM/yyyy hh:mm:ss tt}";

            _ = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: textMsg,
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken);
        }
}