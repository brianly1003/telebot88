using TeleBot.ServiceBot.Interfaces;
using TeleBot.ServiceBot;
using TeleBot.ServiceBot.Utils;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TeleBot.Services
{
    public class UpdateHandlers
    {
        private readonly ILogger<UpdateHandlers> _logger;
        private readonly ITelegramBotClient _botClient;
        private readonly IUserStateManager _userStateManager;
        private readonly ICommandFactory _commandFactory;

        private readonly Dictionary<long, bool> _hasLoadedForUser = new();

        public UpdateHandlers(ITelegramBotClient botClient, ILogger<UpdateHandlers> logger,
            IUserStateManager userStateManager, ICommandFactory commandFactory)
        {
            _botClient = botClient;
            _logger = logger;
            _userStateManager = userStateManager;
            _commandFactory = commandFactory;
        }

        public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
        {
            var me = await _botClient.GetMeAsync(cancellationToken: cancellationToken);

            try
            {
                var chatId = update.Message?.Chat.Id ?? update.CallbackQuery?.Message?.Chat.Id;
                if (chatId == null) return;

                await InitialLoading(update.Message, cancellationToken);

                if (update.Message is { } message)
                {
                    if (message.Text is { } messageText || message.Contact != null)
                    {
                        await HandleMessageUpdate(me, message, cancellationToken);
                    }
                }
                else if (update.CallbackQuery is { } callbackQuery)
                {
                    await HandleCallbackQueryUpdate(me, callbackQuery, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex, cancellationToken);
            }
        }

        #region Private Methods

        private async Task HandleMessageUpdate(User me, Message message, CancellationToken cancellationToken)
        {
            var chatId = message.Chat.Id;

            if (_userStateManager.TryGetUserState(chatId, out var currentCommand))
            {
                if (currentCommand != null)
                {
                    await currentCommand.UserInteractive(_botClient, message, cancellationToken);
                    return;
                }
            }

            var command = _commandFactory.CreateCommand(me.Username, message);
            if (command != null)
            {
                await command.ExecuteAsync(_botClient, message, null, cancellationToken);
            }
        }

        private async Task HandleCallbackQueryUpdate(User me, CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            var callbackDataString = callbackQuery.Data ?? string.Empty;
            var callbackDataObject = DataExtensions.BuildCallbackData(callbackDataString);

            var commandText = callbackDataObject == null ? callbackDataString : callbackDataObject.CommandName;
            var command = _commandFactory.CreateCommand(me.Username, messageText: commandText);
            if (command != null)
            {
                await command.ExecuteAsync(_botClient, callbackQuery.Message, callbackQuery, cancellationToken);
            }
        }

        private async Task InitialLoading(Message? message, CancellationToken cancellationToken)
        {
            if (message == null || message.Chat.Type != ChatType.Private)
                return;

            var chatId = message.Chat.Id;

            if (!_hasLoadedForUser.TryGetValue(chatId, out var hasLoaded) || !hasLoaded)
            {
                var gameUrl = "https://8xr.io/telegram/endlessSiege?accessToken=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjY1MTU0N2YzNTUzZjAzZmI1OWU3YjliZCIsInJvbGUiOiJ1c2VyIiwiaWF0IjoxNjk1ODkzNzA3LCJhdWQiOiI4eHIiLCJpc3MiOiI4eHIifQ.-KETmwuNCOEXeMzSmV3kgaXKcJiLDEAUglh3qfofA1M&inlineMessageId=&chatId=1142101264&messageId=313509#tgShareScoreUrl=tg%3A%2F%2Fshare_game_score%3Fhash%3DXWUVlRi4bm1gtmzc5kM9dHcBRn0Z1X0Ginf8jfpt1lWayMQSd_J0m0oCQ675wX04";
                //var gameUrl = "https://lobby.sgplayfun.com/touch/spadenew/20210715P/games/pokerways/index.jsp?game=S-PW03&token=804a09f9c1f85d1f497491a8512af6a5389be0b1c0d4830e6b6f660086ac7e38531ae97221253343b7bad8d0dc3ebf38e914409c21ba59ad35d10b46ba5998df&m=MB88&c=MYR&language=en_US&fun=true&type=web";

                //await _botClient.SetChatMenuButtonAsync(chatId: chatId, menuButton: new MenuButtonWebApp
                //{
                //    WebApp = new WebAppInfo
                //    {
                //        Url = gameUrl
                //    },
                //    Text = TextCommands.Menu
                //}, cancellationToken: cancellationToken);

                await _botClient.SetChatMenuButtonAsync(chatId: chatId, menuButton: new MenuButtonCommands(), cancellationToken: cancellationToken);

                _hasLoadedForUser[chatId] = true;
            }
        }

        private static Task HandleErrorAsync(Exception exception, CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(errorMessage);
            return Task.CompletedTask;
        }
        #endregion
    }
}
