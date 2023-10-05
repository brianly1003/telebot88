using TeleBot.ServiceBot.Constants;
using TeleBot.ServiceBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleBot.ServiceBot.Commands
{
    public class StartCommand : BaseCommand
    {
        public override string Name => CommandNames.StartCommand;
        public override string CommandText => TextCommands.Start;

        public StartCommand(ICommandHandler commandHandler, IUserStateManager userStateManager) : base(commandHandler, userStateManager)
        {
        }

        public override async Task ExecuteAsync(ITelegramBotClient botClient, Message? message, CallbackQuery? callback, CancellationToken cancellationToken)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
                new KeyboardButton[] { new(TextCommands.ShareContact) { RequestContact = true } },
                new KeyboardButton[] { TextCommands.AboutUs },
            })
            {
                ResizeKeyboard = true
            };

            var chatId = message.Chat.Id;
            var fullName = $"{message.Chat.FirstName} {message.Chat.LastName}";

            var _ = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: $"Hi {fullName}, please share contact to register.",
                replyMarkup: replyKeyboardMarkup,
                cancellationToken: cancellationToken);
        }
    }
}
