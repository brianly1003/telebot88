using TeleBot.ServiceBot.Constants;
using TeleBot.ServiceBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TeleBot.ServiceBot.Commands
{
    public class ShareContactCommand : BaseCommand
    {
        private readonly IUserStateManager _userStateManager;
        private readonly ICommandHandler _commandHandler;

        public override string Name => nameof(ShareContactCommand);
        public override string CommandText => TextCommands.ShareContact;

        public ShareContactCommand(IUserStateManager userStateManager, ICommandHandler commandHandler) : base(commandHandler, userStateManager)
        {
            _userStateManager = userStateManager;
            _commandHandler = commandHandler;
        }

        public override async Task ExecuteAsync(ITelegramBotClient botClient, Message? message, CallbackQuery? callback, CancellationToken cancellationToken)
        {
            var chatId = message.Chat.Id;

            // Set the user state
            _userStateManager.SetUserState(chatId, this);

            await botClient.SendTextMessageAsync(chatId, $"Please enter agent / referrer code. Enter 'No' to cancel if don't have any.", cancellationToken: cancellationToken);
        }

        public override async Task HandleResponse(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            var chatId = message.Chat.Id;
            if (message.Text?.Trim().ToLower() == "no")
            {
                // Reset the user state and call MainCommand
                _userStateManager.RemoveUserState(chatId);
                // ... call MainCommand
                await botClient.SendTextMessageAsync(chatId, "Register success!", cancellationToken: cancellationToken);

                // Execute MainCommand via CommandHandler
                var mainCommand = _commandHandler.GetBotCommand(CommandNames.MainCommand);
                if (mainCommand != null)
                {
                    await mainCommand.ExecuteAsync(botClient, message, null, cancellationToken);
                }
            }
            else
            {
                // Validate agent / referrer code here and send back message.
                bool isValid = ValidateCode(message.Text); // Assuming a method to validate the code.
                if (isValid)
                {
                    await botClient.SendTextMessageAsync(chatId, $"Valid Code! Proceeding...", cancellationToken: cancellationToken);

                    // Execute MainCommand via CommandHandler
                    var mainCommand = _commandHandler.GetBotCommand(CommandNames.MainCommand);
                    if (mainCommand != null)
                    {
                        await mainCommand.ExecuteAsync(botClient, message, null, cancellationToken);
                    }
                }
                else
                {
                    await botClient.SendTextMessageAsync(chatId, $"Invalid Code! Please enter a valid agent / referrer code or enter 'No' to cancel.", cancellationToken: cancellationToken);
                }
            }
        }

        private static bool ValidateCode(string? code)
        {
            // Implement your validation logic here.
            return true; // Placeholder
        }
    }
}
