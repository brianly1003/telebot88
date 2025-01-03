using CW88.TeleBot.ServiceBot.Constants;
using CW88.TeleBot.ServiceBot.Interfaces;
using CW88.TeleBot.ServiceBot.Models;
using CW88.TeleBot.ServiceBot.Utils;
using CW88.TeleBot.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CW88.TeleBot.ServiceBot.Commands.Promotion;

public class PromotionCommand(
    ICommandHandler commandHandler,
    IUserStateManager userStateManager,
    IPromotionService promotionService)
    : BaseCommand(commandHandler, userStateManager)
{
    public override string Name => CommandNames.PromotionCommand;
    public override string CommandText => TextCommands.Promotion;

    public override async Task ExecuteAsync(ITelegramBotClient botClient, Message? message,
        CallbackQuery? callbackQuery, InlineQuery? inlineQuery, CancellationToken cancellationToken)
    {
        var chatId = message?.Chat.Id;
        if (chatId == null) return;

        var messageId = message?.MessageId ?? -1;

        // Get Data
        var promotionList = await promotionService.GetPromotions();

        // Prepare message
        var textMsg = "Please select" +
                      $"\n\nUpdated On: {DateTime.Now:d/MM/yyyy hh:mm:ss tt}";

        if (callbackQuery == null)
        {
            var inlineKeyboard = GenerateInlineKeyboard(promotionList.Select(s => s.PromotionType).ToList(),
                type: PromotionEnum.PromotionType, columns: 2);

            _ = await botClient.SendMessage(
                chatId: chatId,
                text: textMsg,
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken);
        }
        else
        {
            var callbackData = DataExtensions.BuildCallbackData(callbackQuery.Data ?? string.Empty);
            var promotions =
                promotionList.FirstOrDefault(i => i.PromotionType == callbackData?.CommandText)?.PromotionItems ?? [];
            var inlineKeyboard = GenerateInlineKeyboard(promotions, type: PromotionEnum.Promotion, columns: 2);

            _ = await botClient.EditMessageTextAsync(
                chatId: chatId,
                messageId: messageId,
                text: textMsg,
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken);
        }
    }

    #region Private Methods

    private static InlineKeyboardMarkup GenerateInlineKeyboard(ICollection<string> promotionList,
        PromotionEnum type, int columns)
    {
        var rows = new List<InlineKeyboardButton[]>();

        if (promotionList.Count % columns != 0)
        {
            promotionList.Add(""); // Add an empty bank item
        }

        for (var i = 0; i < promotionList.Count; i += columns)
        {
            var chunk = promotionList.Skip(i).Take(columns).ToList();
            var commandName = type == PromotionEnum.PromotionType
                ? CommandNames.PromotionCommand
                : CommandNames.PromotionDetailCommand;

            var buttons = chunk.Select(promotion => string.IsNullOrWhiteSpace(promotion)
                ? InlineKeyboardButton.WithCallbackData(TextCommands.Empty)
                : InlineKeyboardButton.WithCallbackData(promotion,
                    DataExtensions.SerializeCallbackData(new CallbackData(promotion, commandName)))).ToArray();

            rows.Add(buttons);
        }

        // Adding the Back command to the end
        rows.Add(new[] { InlineKeyboardButton.WithCallbackData(TextCommands.Cancel) });

        return new InlineKeyboardMarkup(rows);
    }

    public enum PromotionEnum
    {
        Promotion,
        PromotionType
    }

    #endregion
}