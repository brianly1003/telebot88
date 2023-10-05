using TeleBot.ServiceBot.Constants;
using TeleBot.ServiceBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleBot.ServiceBot.Commands
{
    public class PlayCommand : BaseCommand
    {
        public override string Name => CommandNames.PlayCommand;
        public override string CommandText => TextCommands.Play;

        public PlayCommand(ICommandHandler commandHandler, IUserStateManager userStateManager) : base(commandHandler, userStateManager)
        {
        }

        public override async Task ExecuteAsync(ITelegramBotClient botClient, Message? message,
            CallbackQuery? callbackQuery, CancellationToken cancellationToken)
        {
            var chatId = message?.Chat.Id;
            if (chatId == null) return;

            var gameUrl = "https://lobby.sgplayfun.com/touch/spadenew/20210715P/games/pokerways/index.jsp?game=S-PW03&token=804a09f9c1f85d1f497491a8512af6a5389be0b1c0d4830e6b6f660086ac7e38531ae97221253343b7bad8d0dc3ebf38e914409c21ba59ad35d10b46ba5998df&m=MB88&c=MYR&language=en_US&fun=true&type=web";
            //var gameUrl = "https://8xr.io/telegram/endlessSiege?accessToken=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjY1MTU0N2YzNTUzZjAzZmI1OWU3YjliZCIsInJvbGUiOiJ1c2VyIiwiaWF0IjoxNjk1ODkzNzA3LCJhdWQiOiI4eHIiLCJpc3MiOiI4eHIifQ.-KETmwuNCOEXeMzSmV3kgaXKcJiLDEAUglh3qfofA1M&inlineMessageId=&chatId=1142101264&messageId=313509#tgShareScoreUrl=tg%3A%2F%2Fshare_game_score%3Fhash%3DXWUVlRi4bm1gtmzc5kM9dHcBRn0Z1X0Ginf8jfpt1lWayMQSd_J0m0oCQ675wX04";

            var webAppUrl = "https://brianly1003.github.io/telegram-web-app-bot-example/index.html";

            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new [] { InlineKeyboardButton.WithWebApp(TextCommands.PlayNow, new WebAppInfo { Url = gameUrl }) },
                new [] { InlineKeyboardButton.WithWebApp(TextCommands.MiniApp, new WebAppInfo { Url = webAppUrl }) },
                new [] { InlineKeyboardButton.WithCallbackData(TextCommands.Earn), InlineKeyboardButton.WithCallbackData(TextCommands.Wallet) },
                new [] { InlineKeyboardButton.WithUrl(TextCommands.JoinCommunity, url: "https://t.me/wtv88site") }
            });

            var htmlText = $"Hi!\n\nWelcome to @Whale, #1 Online Casino and Sportsbook!" +
                           "\n\n🎰 Most Popular Casino Games" +
                            "\n💰Automated & instant withdrawals" +
                            "\n🏦 TON & USDT support!";

            var _ = await botClient.SendAnimationAsync(
                chatId: chatId,
                animation: InputFile.FromUri("https://raw.githubusercontent.com/TelegramBots/book/master/src/docs/video-waves.mp4"),
                caption: htmlText,
                replyMarkup: inlineKeyboard,
                parseMode: ParseMode.Html,
                cancellationToken: cancellationToken);
        }
    }
}
