using W88.TeleBot.ServiceBot.Constants;
using W88.TeleBot.ServiceBot.Interfaces;

namespace W88.TeleBot.ServiceBot;

public class CommandFactory(
    IUserStateManager userStateManager,
    ICommandHandler commandHandler) : ICommandFactory
{
    private readonly IUserStateManager _userStateManager = userStateManager;

    public IBotCommand? CreateCommand(string? botName, Message? message = null, InlineQuery? inlineQuery = null,
        string? commandText = null, string? commandName = null)
    {
            var commandTextResult = GetCommandText(botName, message, commandText);
            var commandNameResult = GetCommandName(message, inlineQuery, commandName);

            // Check blacklist and whitelist
            if (!string.IsNullOrWhiteSpace(commandTextResult))
            {
                if (TextCommands.BlacklistTextCommands.Contains(commandTextResult))
                {
                    return null; // Blocked command, higher priority for blacklist
                }

                if (!TextCommands.WhitelistTextCommands.Contains(commandTextResult))
                {
                    return null; // Command not in whitelist, so it's ignored
                }
            }

            return string.IsNullOrWhiteSpace(commandTextResult) && string.IsNullOrWhiteSpace(commandNameResult)
                ? null
                : commandHandler.GetBotCommand(commandTextResult, commandNameResult);
        }


    private static string GetCommandText(string? botName, Message? message, string? messageText)
    {
            //if (message?.Contact != null)
            //    return TextCommands.ShareContact;

            var commandText = message?.Text ?? messageText ?? string.Empty;

            if (botName != null && commandText.StartsWith($"@{botName} "))
                commandText = commandText[(botName.Length + 2)..].Trim();

            return commandText;
        }

    private static string GetCommandName(Message? message, InlineQuery? inlineQuery, string? commandName)
    {
            if (message?.Contact != null)
                return CommandNames.ShareContactCommand;

            if (inlineQuery != null)
            {
                return inlineQuery.Query;
            }

            return !string.IsNullOrWhiteSpace(commandName) ? commandName : string.Empty;
        }
}