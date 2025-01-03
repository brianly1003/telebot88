﻿using Microsoft.Extensions.Localization;
using Telegram.Bot.Types.ReplyMarkups;
using W88.TeleBot.Resources;
using W88.TeleBot.ServiceBot.Constants;
using W88.TeleBot.ServiceBot.Interfaces;

namespace W88.TeleBot.ServiceBot.Commands;

public class StartCommand(
    ICommandHandler commandHandler,
    IUserStateManager userStateManager,
    ILogger<StartCommand> logger,
    IStringLocalizer<SharedResource> localizer) : BaseCommand(commandHandler, userStateManager)
{
    public override string Name => CommandNames.StartCommand;
    public override string CommandText => TextCommands.Start;

    public override async Task ExecuteAsync(ITelegramBotClient botClient, Message? message, CallbackQuery? callback,
        InlineQuery? inlineQuery, CancellationToken cancellationToken)
    {
        var chatId = message?.Chat.Id;
        if (chatId == null) return;

        try
        {
            var languageCode = message?.From?.LanguageCode;
            if (languageCode == null) return;

            SetCulture(languageCode);

            var replyKeyboardMarkup = new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton($"✅ {localizer["Confirm"]}") { RequestContact = true },
            })
            {
                ResizeKeyboard = true
            };

            var textMsg = localizer["Welcome to W88 Casino"].Value.Replace("\\u2705", "\u2705");

            _ = await botClient.SendMessage(
                chatId: chatId!,
                text: textMsg,
                replyMarkup: replyKeyboardMarkup,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            await botClient.SendMessage(chatId,
                localizer["An error occured. Please try again later!"],
                cancellationToken: cancellationToken);
        }
    }
}