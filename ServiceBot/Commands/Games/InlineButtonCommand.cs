using CW88.TeleBot.ServiceBot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using W88.TeleBot.ServiceBot.Constants;
using W88.TeleBot.ServiceBot.Interfaces;
using InputFile = Telegram.Bot.Types.InputFile;

namespace W88.TeleBot.ServiceBot.Commands.Games;

public class InlineButtonCommand(ICommandHandler commandHandler, IUserStateManager userStateManager)
    : BaseCommand(commandHandler, userStateManager)
{
    public override string Name => CommandNames.InlineButtonCommand;
    public override string CommandText => TextCommands.InlineButton;

    public override async Task ExecuteAsync(ITelegramBotClient botClient, Message? message,
        CallbackQuery? callbackQuery, InlineQuery? inlineQuery, CancellationToken cancellationToken)
    {
        var chatId = message?.Chat.Id;
        if (chatId == null) return;

        var gameUrl =
            "https://lobby.sgplayfun.com/touch/spadenew/20210715P/games/pokerways/index.jsp?game=S-PW03&token=804a09f9c1f85d1f497491a8512af6a5389be0b1c0d4830e6b6f660086ac7e38531ae97221253343b7bad8d0dc3ebf38e914409c21ba59ad35d10b46ba5998df&m=MB88&c=MYR&language=en_US&fun=true&type=web";
        var telegramUrl = $"https://t.me/wtv88_bot?game=pokerways";

        var inlineKeyboard = new InlineKeyboardMarkup(new[]
        {
            new[] { InlineKeyboardButton.WithWebApp("Play Poker Ways", new WebAppInfo { Url = gameUrl }) }
        });

        var textMsg = "*Poker Ways*" +
                      "\nRewards await in Transforming Gold Plated Symbols & Bonus Multiplier with cascading wins" +
                      "[\\!](https://github.com/brianly1003/telebot88/raw/main/assets/media/game-photo-640x360.png)";

        //var imageFile = InputFile.FromUri("https://github.com/brianly1003/telebot88/raw/main/assets/media/whale_promo_fixed.mp4");
        var imageFile =
            InputFile.FromUri("https://github.com/brianly1003/telebot88/raw/main/assets/media/game-photo-640x360.png");

        //_ = await botClient.SendAnimation(
        //    chatId: chatId,
        //    animation: imageFile,
        //    caption: textMsg,
        //    replyMarkup: inlineKeyboard,
        //    parseMode: ParseMode.Html,
        //    cancellationToken: cancellationToken);

        _ = await botClient.SendMessage(
            chatId: chatId,
            text: textMsg,
            replyMarkup: inlineKeyboard,
            parseMode: ParseMode.MarkdownV2,
            cancellationToken: cancellationToken);
    }
}