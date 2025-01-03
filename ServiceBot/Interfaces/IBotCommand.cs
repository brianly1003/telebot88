namespace W88.TeleBot.ServiceBot.Interfaces;

public interface IBotCommand : IHandleState
{
    string Name { get; }

    /// <summary>
    /// Gets the text representation of the command. 
    /// Use this when there is only a single command associated with this command handler.<br/>
    /// Eg: <br/>
    /// public override string CommandText => "/start";
    /// </summary>
    string CommandText { get; }

    /// <summary>
    /// Gets an array of command strings. 
    /// Use this when there are multiple commands associated with this command handler.<br/>
    /// Eg: <br/>
    /// public override string[] Commands { get; } = { "/start", "StartCommand" };
    /// </summary>
    string[] Commands { get; }

    Task ExecuteAsync(ITelegramBotClient botClient, Message? message, CallbackQuery? callbackQuery, InlineQuery? inlineQuery, CancellationToken cancellationToken);

    Task UserInteractive(ITelegramBotClient botClient, Message? message, CancellationToken cancellationToken);
}