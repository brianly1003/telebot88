﻿using CW88.TeleBot.ServiceBot.Constants;
using CW88.TeleBot.ServiceBot.Interfaces;
using CW88.TeleBot.ServiceBot.Models;
using CW88.TeleBot.ServiceBot.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace CW88.TeleBot.ServiceBot.Commands.Common;

public class CopyCommand(ICommandHandler commandHandler, IUserStateManager userStateManager)
    : BaseCommand(commandHandler,
        userStateManager)
{
    public override string Name => CommandNames.CopyCommand;
    public override string CommandText => TextCommands.Copy;

    public override async Task ExecuteAsync(ITelegramBotClient botClient, Message? message,
        CallbackQuery? callbackQuery, InlineQuery? inlineQuery, CancellationToken cancellationToken)
    {
        var chatId = message?.Chat.Id;
        if (chatId == null) return;

        const string textMsg = "`https://t.me/wtv88_bot`";

        var inlineKeyboard = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(TextCommands.Back,
                    DataExtensions.SerializeCallbackData(new CallbackData(TextCommands.Back,
                        CommandNames.EarnCommand)))
            }
        });

        if (callbackQuery == null)
        {
            _ = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: textMsg,
                replyMarkup: inlineKeyboard,
                parseMode: ParseMode.MarkdownV2,
                cancellationToken: cancellationToken);
        }
        else
        {
            var messageId = message?.MessageId ?? -1;

            _ = await botClient.EditMessageTextAsync(
                chatId: chatId,
                messageId: messageId,
                text: textMsg,
                replyMarkup: inlineKeyboard,
                parseMode: ParseMode.MarkdownV2,
                cancellationToken: cancellationToken);
        }
    }
}