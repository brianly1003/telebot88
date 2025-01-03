namespace CW88.TeleBot.ServiceBot.Constants;

public static class TextCommands
{

    public const string Start = "/start";
    public const string Play = "/play";
    public const string Ping = "/ping";

    public const string ChatMode = "/chat";
    public const string ExitChatMode = "/exit";

    public const string OpenMiniApp = "/miniapp";
    public const string ChooseGames = "/games";
    public const string LaunchGame = "/launchgame";
    public const string InlineButton = "/inlinebutton";

    public const string AboutUs = "ℹ️ About Us ℹ️";
    public const string ShareContact = "📞 Share Contact 📞";
    public const string ConfirmRegistration = "✅ Confirm";

    public const string Menu = "Menu";
    public const string Empty = " ";
    public const string Cancel = "Cancel";
    public const string Back = "« Back";

    // GameCommand
    public const string PlayNow = "🔥 Play Now 🔥";
    public const string MiniApp = "📱 Mini App 📱";
    public const string Earn = "💰 Earn 💰";
    public const string Wallet = "👛 Wallet 👛";
    public const string JoinCommunity = "💭 Join Community 💭";

    // MainCommand
    public const string Chat = "💭 Chat";
    public const string Games = "🎮 Games";
    public const string Deposit = "⏩ Deposit";
    public const string Transfer = "🔁 Transfer";
    public const string Withdraw = "⏪ Withdraw";
    public const string Balance = "💰 Balance";
    public const string Profile = "👤 Profile";
    public const string Bank = "🏦 Bank";
    public const string Promotion = "🎁 Promotion";
    public const string History = "📋 History";
    public const string ContactUs = "📞 Contact Us";
    public const string Setting = "⚙️ Setting";

    // GameCommand
    public const string Sport = "⚽ Sport";
    public const string Casino = "🎲 Casino";
    public const string Slot = "🎰 Slot";
    public const string Fishing = "🐬 Fishing";
    public const string Lottery = "🎱 Lottery";
    public const string ESport = "⛳ ESport";
    public const string CardnBoard = "🃏 Card & Board";
    public const string CockFighting = "🐔 Cock Fighting";

    // DepositCommand
    public const string OnlineBanking = "Online Banking";
    public const string PaymentGateway = "Payment Gateway";

    // BankCommand
    public const string AddBankAccount = "Add Bank Account";

    // OnlineBankingCommand
    public const string UnionBank = "UnionBank";
    public const string BPIBank = "BPIBank";
    public const string MetroBank = "MetroBank";

    // PlayCommand Group
    public const string Share = "Share";
    public const string Copy = "Copy";
    public const string Stats = "Stats";
    public const string Guide = "📖 Guide";

    public static readonly List<string> AllCommands =
    [
        Start,
        Play,
        Ping,

        AboutUs,
        ShareContact,

        Cancel,
        Back,

        Games,
        Deposit,

        OnlineBanking,
        PaymentGateway
    ];

    public static readonly List<string> WhitelistTextCommands = [Start, Ping];

    public static readonly List<string> BlacklistTextCommands = [];
}