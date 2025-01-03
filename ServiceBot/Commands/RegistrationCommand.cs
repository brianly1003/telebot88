using CW88.TeleBot.ServiceBot.Constants;
using CW88.TeleBot.ServiceBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CW88.TeleBot.ServiceBot.Commands;

public class RegistrationCommand(ICommandHandler commandHandler, IUserStateManager userStateManager)
    : BaseCommand(commandHandler, userStateManager)
{
    public override string Name => CommandNames.RegistrationCommand;

    public override async Task ExecuteAsync(ITelegramBotClient botClient, Message? message,
        CallbackQuery? callbackQuery,
        InlineQuery? inlineQuery, CancellationToken cancellationToken)
    {
            var chatId = message?.Chat.Id;
            if (chatId == null) return;

            var replyKeyboard = new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton(TextCommands.ConfirmRegistration) { RequestContact = true },
            })
            {
                ResizeKeyboard = true
            };

            const string textMsg = "To proceed with the registration, kindly share your contact number. Tap \"\u2705 Confirm\" or \u2328\ufe0f icon in the message input if you don't see the button";

            _ = await botClient.SendTextMessageAsync(
                chatId: chatId!,
                text: textMsg,
                replyMarkup: replyKeyboard,
                cancellationToken: cancellationToken);
        }
}