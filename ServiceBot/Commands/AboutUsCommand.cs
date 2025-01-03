using CW88.TeleBot.ServiceBot;
using Telegram.Bot.Types.ReplyMarkups;
using W88.TeleBot.ServiceBot.Constants;
using W88.TeleBot.ServiceBot.Interfaces;

namespace W88.TeleBot.ServiceBot.Commands;

public class AboutUsCommand(ICommandHandler commandHandler, IUserStateManager userStateManager)
    : BaseCommand(commandHandler, userStateManager)
{
    public override string Name => CommandNames.AboutUsCommand;
    public override string CommandText => TextCommands.AboutUs;

    public override async Task ExecuteAsync(ITelegramBotClient botClient, Message? message, CallbackQuery? callback, InlineQuery? inlineQuery, CancellationToken cancellationToken)
    {
        var chatId = message?.Chat.Id;
        if (chatId == null) return;

        InlineKeyboardMarkup inlineKeyboard = new(new[]
        {
            InlineKeyboardButton.WithCallbackData(TextCommands.Cancel),
        });

        var textMsg =
            "Welcome to our Telegram Casino Bot! We are a cutting-edge platform that brings the thrill of casino gaming right to your fingertips within the Telegram messaging app." +
            "\n\nOur casino bot is designed to provide a seamless and convenient gambling experience for Telegram users. Whether you're a seasoned player or new to the world of online casinos, our bot offers a wide range of exciting games and features to cater to every level of expertise." +
            "\n\nWith our Telegram Casino Bot, you can enjoy popular casino games such as slots, blackjack, roulette, and poker, all from the comfort of your Telegram chat. We've carefully selected top-quality games from renowned game providers to ensure an immersive and entertaining gaming experience." +
            $"\n\nUpdated On: {DateTime.Now:d/MM/yyyy hh:mm:ss tt}";

        _ = await botClient.SendMessage(
            chatId: chatId,
            text: textMsg,
            replyMarkup: inlineKeyboard,
            cancellationToken: cancellationToken);
    }
}