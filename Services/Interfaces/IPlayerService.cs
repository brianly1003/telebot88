using CI24.Apps.NetCore.HttpApiFacade.EzPortal;
using CW88.TeleBot.Model.Player;
using Fuhrer.Common;

namespace CW88.TeleBot.Services.Interfaces;

public interface IPlayerService
{
    Task<OpResults<RegisterPlayer.RegisterPlayerResult>> RegisterPlayer(RegisterPlayerArgs args);

    Task<OpResults<GetPlayerByTelegramUID.PlayerInfo>> GetPlayerByTelegramUID(string telegramUID, string mobileNumber);

    Task<OpResults<string>> OneTimeLoginLinkFor(string uid);
}