namespace W88.TeleBot.Model;

public class BotConfig
{
    public string BotName { get; init; } = string.Empty;
    public string BotToken { get; init; } = string.Empty;
    public string SecretToken { get; init; } = string.Empty;
    public string HostAddress { get; init; } = string.Empty;
    public string BotUrl { get; set; } = string.Empty;
    public bool UseWebhook { get; init; } = false;
}

public class CoreConfig
{
    public static readonly string Configuration = nameof(CoreConfig);

    // List of bot configurations
    public List<BotConfig> Bots { get; init; } = [];
    public string ChatUrl { get; init; } = default!;
    public string MiniAppUrl { get; init; } = default!;
    public string ValidationKey { get; init; } = default!;
    public string JwtPublicKey { get; init; } = default!;
}
