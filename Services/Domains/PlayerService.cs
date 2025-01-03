using CW88.TeleBot.Model.Player;
using CW88.TeleBot.Services.Interfaces;

namespace CW88.TeleBot.Services.Domains;

public class PlayerService() : IPlayerService
{
    // public async Task<OpResults<RegisterPlayer.RegisterPlayerResult>> RegisterPlayer(RegisterPlayerArgs args)
    // {
    //     var registerUser = await commandRunner.InvokeHttpApiAsync(
    //         new RegisterPlayer
    //         {
    //             AlwaysRegister = true,
    //             IsPasswordDeliveredManually = false,
    //             MobileNumber = args.MobileNumber,
    //             TelegramUID = args.TelegramUID,
    //             //AnalyticsTrackingString = trackingString,
    //             //TrafficSource = trafficSource
    //         },
    //         null
    //     );
    //
    //     return registerUser;
    // }
    //
    // public async Task<OpResults<GetPlayerByTelegramUID.PlayerInfo>> GetPlayerByTelegramUID(string telegramUID, string mobileNumber)
    // {
    //     var player = await commandRunner.InvokeHttpApiAsync(
    //         new GetPlayerByTelegramUID
    //         {
    //             TelegramUID = telegramUID,
    //             MobileNumber = mobileNumber
    //         },
    //         null
    //     );
    //
    //     return player;
    // }
    //
    // public async Task<OpResults<string>> OneTimeLoginLinkFor(string uid)
    // {
    //     var result = await commandRunner.InvokeHttpApiAsync(
    //         new OneTimeLoginLinkFor
    //         {
    //             Uid = uid
    //         },
    //         null
    //     );
    //
    //     return result;
    // }
}