using CW88.TeleBot.ServiceBot.Constants;
using CW88.TeleBot.ServiceBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineQueryResults;

namespace CW88.TeleBot.ServiceBot.Commands.Games;

public class GamesCommand : BaseCommand
{
    public override string Name => CommandNames.GamesCommand;
    public override string CommandText => string.Empty;

    public GamesCommand(ICommandHandler commandHandler, IUserStateManager userStateManager) : base(commandHandler, userStateManager)
    {
    }

    public override async Task ExecuteAsync(ITelegramBotClient botClient, Message? message, CallbackQuery? callbackQuery, InlineQuery? inlineQuery, CancellationToken cancellationToken)
    {
        var miniAppUrl = $"https://t.me/wtv88_bot/miniapp";
        var gameUrl = "https://lobby.sgplayfun.com/touch/spadenew/20210715P/games/pokerways/index.jsp?game=S-PW03&token=804a09f9c1f85d1f497491a8512af6a5389be0b1c0d4830e6b6f660086ac7e38531ae97221253343b7bad8d0dc3ebf38e914409c21ba59ad35d10b46ba5998df&m=MB88&c=MYR&language=en_US&fun=true&type=web";
        var telegramUrl = $"https://t.me/wtv88_bot?game=pokerways";

        if (inlineQuery != null)
        {
            var inlineQueryId = inlineQuery?.Id;
            if (string.IsNullOrEmpty(inlineQueryId)) return;

            var results = new List<InlineQueryResultGame>
            {
                new("1", "pokerways"),
                new("2", "classicfruits")
            };

            //var results = new List<InlineQueryResultArticle>
            //{
            //    new(
            //        id: "1", // Unique ID for the result
            //        title: "Game [Browser]", // Title displayed to the user
            //        inputMessageContent: new InputTextMessageContent(
            //            $"[Open Game]({telegramUrl})") // Message content that uses Markdown to display a clickable link
            //    )
            //    {
            //        Description =
            //            "Tap to open the Game from external browser", // Additional description for the user
            //        //ThumbUrl = "https://example.com/thumbnail.jpg", // Optional: a thumbnail URL for the result
            //        ReplyMarkup =
            //            new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl("Open External Browser", gameUrl)) // Optional: a button the user can tap to open the link
            //    },
            //    new(
            //        id: "2", // Unique ID for the result
            //        title: "MiniApp [Telegram]", // Title displayed to the user
            //        inputMessageContent: new InputTextMessageContent(
            //            $"[Open Mini-App]({miniAppUrl})") // Message content that uses Markdown to display a clickable link
            //    )
            //    {
            //        Description =
            //            "Tap to open the mini-app within Telegram", // Additional description for the user
            //        //ThumbUrl = "https://example.com/thumbnail.jpg", // Optional: a thumbnail URL for the result
            //        ReplyMarkup =
            //            new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl("Open within Telegram",
            //                miniAppUrl)) // Optional: a button the user can tap to open the link
            //    },
            //};

            // Respond to the inline query with the results
            await botClient.AnswerInlineQueryAsync(
                inlineQueryId: inlineQueryId,
                results: results,
                isPersonal: true, // Results are specific to the user
                cacheTime: 0, // The result cache time in seconds
                cancellationToken: cancellationToken
            );
        }
        else
        {
            var chatId = message?.Chat.Id;
            if (chatId == null) return;

            // Custom button
            //var playButton = InlineKeyboardButton.WithCallBackGame("Play Game");
            //var inlineKeyboard = new InlineKeyboardMarkup(playButton);

            _ = await botClient.SendGameAsync(
                chatId: (long)chatId,
                gameShortName: "pokerways",
                replyMarkup: null,
                cancellationToken: cancellationToken);
        }
    }
}