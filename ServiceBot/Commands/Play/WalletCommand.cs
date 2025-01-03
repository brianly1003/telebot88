﻿using CW88.TeleBot.ServiceBot.Constants;
using CW88.TeleBot.ServiceBot.Interfaces;
using CW88.TeleBot.ServiceBot.Models;
using CW88.TeleBot.ServiceBot.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace CW88.TeleBot.ServiceBot.Commands.Play;

public class WalletCommand(ICommandHandler commandHandler, IUserStateManager userStateManager)
    : BaseCommand(commandHandler, userStateManager)
{
    public override string Name => TextCommands.Wallet;
    public override string CommandText => CommandNames.WalletCommand;

    public override async Task ExecuteAsync(ITelegramBotClient botClient, Message? message,
        CallbackQuery? callbackQuery, InlineQuery? inlineQuery, CancellationToken cancellationToken)
    {
            var chatId = message?.Chat.Id;
            if (chatId == null) return;

            var messageId = message?.MessageId ?? -1;

            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(TextCommands.Deposit),
                    InlineKeyboardButton.WithCallbackData(TextCommands.Withdraw)
                },
                new[] { InlineKeyboardButton.WithUrl(TextCommands.Guide, "https://whale.io/") },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(TextCommands.Back,
                        DataExtensions.SerializeCallbackData(new CallbackData(TextCommands.Back,
                            CommandNames.PlayCommand)))
                }
            });

            const string htmlText = $"💵 Wallet" +
                                    "\n\nBalance: 0.00 TON";

            if (callbackQuery != null)
            {
                await botClient.DeleteMessageAsync(chatId: chatId, messageId: messageId,
                    cancellationToken: cancellationToken);
            }

            _ = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: htmlText,
                replyMarkup: inlineKeyboard,
                parseMode: ParseMode.Html,
                cancellationToken: cancellationToken);
        }
}