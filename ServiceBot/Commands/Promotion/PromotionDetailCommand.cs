using CW88.TeleBot.ServiceBot.Constants;
using CW88.TeleBot.ServiceBot.Interfaces;
using CW88.TeleBot.ServiceBot.Models;
using CW88.TeleBot.ServiceBot.Utils;
using CW88.TeleBot.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace CW88.TeleBot.ServiceBot.Commands.Promotion;

public class PromotionDetailCommand(
    ICommandHandler commandHandler,
    IUserStateManager userStateManager,
    IPromotionService promotionService)
    : BaseCommand(commandHandler, userStateManager)
{
    public override string Name => CommandNames.PromotionDetailCommand;
    public override string CommandText => string.Empty;

    public override async Task ExecuteAsync(ITelegramBotClient botClient, Message? message,
        CallbackQuery? callbackQuery, InlineQuery? inlineQuery, CancellationToken cancellationToken)
    {
            var chatId = message?.Chat.Id;
            if (chatId == null) return;

            var messageId = message?.MessageId ?? -1;

            // Get Data
            var callbackData = DataExtensions.BuildCallbackData(callbackQuery.Data ?? string.Empty);
            var promotionDetail = await promotionService.GetPromotionDetail(callbackData.CommandText);

            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(TextCommands.Back,
                        DataExtensions.SerializeCallbackData(new CallbackData(callbackData.CommandText,
                            CommandNames.PromotionCommand)))
                }
            });

            var textMsg =
                $"{promotionDetail.Content}" +
                $"\n\nUpdated On: {DateTime.Now:d/MM/yyyy hh:mm:ss tt}";

            _ = await botClient.EditMessageTextAsync(
                chatId: chatId,
                messageId: messageId,
                text: textMsg,
                parseMode: ParseMode.MarkdownV2,
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken);
        }
}