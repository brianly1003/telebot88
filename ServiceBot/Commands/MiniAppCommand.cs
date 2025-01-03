using CW88.TeleBot.ServiceBot.Constants;
using CW88.TeleBot.ServiceBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

namespace CW88.TeleBot.ServiceBot.Commands;

public class MiniAppCommand(ICommandHandler commandHandler, IUserStateManager userStateManager)
    : BaseCommand(commandHandler, userStateManager)
{
    public override string Name => CommandNames.MiniAppCommand;
    public override string CommandText => TextCommands.OpenMiniApp;

    public override async Task ExecuteAsync(ITelegramBotClient botClient, Message? message, CallbackQuery? callbackQuery, InlineQuery? inlineQuery, CancellationToken cancellationToken)
    {
        var miniAppUrl = $"https://t.me/wtv88_bot/miniapp";

        if (inlineQuery != null)
        {
            var inlineQueryId = inlineQuery?.Id;
            if (string.IsNullOrEmpty(inlineQueryId)) return;

            var results = new List<InlineQueryResultArticle>
            {
                new(
                    id: "1", // Unique ID for the result
                    title: "MiniApp [Browser]", // Title displayed to the user
                    inputMessageContent: new InputTextMessageContent($"[Open Mini-App]({miniAppUrl})") // Message content that uses Markdown to display a clickable link
                )
                {
                    Description = "Tap to open the mini-app from external browser", // Additional description for the user
                    //ThumbUrl = "https://example.com/thumbnail.jpg", // Optional: a thumbnail URL for the result
                    ReplyMarkup = new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl("Open External Browser", miniAppUrl)) // Optional: a button the user can tap to open the link
                },
                new(
                    id: "2",
                    title: "MiniApp [Telegram]",
                    inputMessageContent: new InputTextMessageContent(miniAppUrl)
                )
                //new(
                //    id: "2",
                //    title: "MiniApp [Telegram]",
                //    inputMessageContent: new InputTextMessageContent($"[Open Mini-App]({miniAppUrl})") 
                //)
                //{
                //    Description = "Tap to open the mini-app within Telegram", 
                //    //ThumbUrl = "https://example.com/thumbnail.jpg",
                //    //ReplyMarkup = new InlineKeyboardMarkup(InlineKeyboardButton.WithWebApp("Open Within Telegram", new WebAppInfo { Url = miniAppUrl}))
                //}

            };

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

            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[] { InlineKeyboardButton.WithUrl("MiniApp", miniAppUrl) }
            });


            var textMsg = @$"[🎮]({miniAppUrl})";

            _ = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: textMsg,
                replyMarkup: null,
                parseMode: ParseMode.MarkdownV2,
                cancellationToken: cancellationToken);

            //var imageFile = InputFile.FromUri("https://github.com/brianly1003/telebot88/raw/main/assets/media/ezgif.com-video-to-mp4.mp4");

            //_ = await botClient.SendAnimationAsync(
            //    chatId: chatId,
            //    animation: imageFile,
            //    caption: textMsg,
            //    replyMarkup: inlineKeyboard,
            //    parseMode: ParseMode.Html,
            //    cancellationToken: cancellationToken);
        }
    }
}