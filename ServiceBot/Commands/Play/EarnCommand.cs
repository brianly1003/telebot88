using CW88.TeleBot.ServiceBot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using W88.TeleBot.ServiceBot.Constants;
using W88.TeleBot.ServiceBot.Interfaces;
using W88.TeleBot.ServiceBot.Models;
using W88.TeleBot.ServiceBot.Utils;

namespace W88.TeleBot.ServiceBot.Commands.Play;

public class EarnCommand(ICommandHandler commandHandler, IUserStateManager userStateManager)
    : BaseCommand(commandHandler,
        userStateManager)
{
    public override string Name => CommandNames.EarnCommand;
    public override string CommandText => TextCommands.Earn;

    public override async Task ExecuteAsync(ITelegramBotClient botClient, Message? message,
        CallbackQuery? callbackQuery, InlineQuery? inlineQuery, CancellationToken cancellationToken)
    {
            var chatId = message?.Chat.Id;
            if (chatId == null) return;

            var messageId = message?.MessageId ?? -1;

            var webAppUrl = $"https://whale-tg-cdn-04.b-cdn.net/referrals";

            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithSwitchInlineQueryChosenChat(TextCommands.Share,
                        new SwitchInlineQueryChosenChat
                            { AllowGroupChats = true, AllowChannelChats = true, AllowUserChats = true }),
                    InlineKeyboardButton.WithCallbackData(TextCommands.Copy)
                },
                new[] { InlineKeyboardButton.WithWebApp(TextCommands.Stats, new WebAppInfo { Url = webAppUrl }) },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(TextCommands.Back,
                        DataExtensions.SerializeCallbackData(new CallbackData(TextCommands.Back,
                            CommandNames.PlayCommand)))
                }
            });

            const string htmlText = $"Refer-a-Friend and earn with @whale!" +
                                    "\n\n💰 You get 10% from house edge from every direct referral" +
                                    "\n💰 and 1% for every tier 2 referral!" +
                                    "\n💰 Earnings paid out monthly";

            if (callbackQuery != null)
            {
                await botClient.DeleteMessage(chatId: chatId, messageId: messageId,
                    cancellationToken: cancellationToken);
            }

            _ = await botClient.SendMessage(
                chatId: chatId,
                text: htmlText,
                replyMarkup: inlineKeyboard,
                parseMode: ParseMode.Html,
                cancellationToken: cancellationToken);
        }
}