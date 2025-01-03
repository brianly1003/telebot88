using Telegram.Bot.Types.ReplyMarkups;
using W88.TeleBot.ServiceBot.Constants;
using W88.TeleBot.ServiceBot.Interfaces;

namespace W88.TeleBot.ServiceBot.Commands.Games;

public class LaunchGameCommand(ICommandHandler commandHandler, IUserStateManager userStateManager)
    : BaseCommand(commandHandler, userStateManager)
{
    public override string Name => CommandNames.LaunchGameCommand;
    public override string CommandText => TextCommands.LaunchGame;

    public override async Task ExecuteAsync(ITelegramBotClient botClient, Message? message,
        CallbackQuery? callbackQuery, InlineQuery? inlineQuery, CancellationToken cancellationToken)
    {
        var callbackQueryId = callbackQuery?.Id;
        if (string.IsNullOrWhiteSpace(callbackQueryId)) return;

        var gameUrl = "";
        var pokerWaysUrl =
            "https://lobby.sgplayfun.com/touch/spadenew/20210715P/games/pokerways/index.jsp?game=S-PW03&token=804a09f9c1f85d1f497491a8512af6a5389be0b1c0d4830e6b6f660086ac7e38531ae97221253343b7bad8d0dc3ebf38e914409c21ba59ad35d10b46ba5998df&m=MB88&c=MYR&language=en_US&fun=true&type=web";
        var classicFruits =
            "https://play.88luckydragon.com/games/slots/202309251715/game/index.html?brand=NEXTSPIN&merchantCode=ZCH169&game=sClaFruit7&language=en_US";
        //var miniAppUrl = $"https://t.me/wtv88_bot/miniapp";

        gameUrl = callbackQuery.GameShortName switch
        {
            "pokerways" => pokerWaysUrl,
            "classicfruits" => classicFruits,
            _ => gameUrl
        };

        // Create an inline keyboard with a single button labeled 'Play Game'
        var playButton = InlineKeyboardButton.WithWebApp("Play Game", new WebAppInfo { Url = gameUrl });
        var inlineKeyboard = new InlineKeyboardMarkup(playButton);

        // Message text inviting the user to play the game
        var messageText = "Click the button below to play the game!";

        // Send the message with the inline keyboard
        await botClient.AnswerCallbackQuery(callbackQueryId: callbackQueryId, url: gameUrl,
            cancellationToken: cancellationToken);
    }
}