using CW88.TeleBot.ServiceBot.Interfaces;
using CW88.TeleBot.ServiceBot.Models;
using CW88.TeleBot.Services.Interfaces;

namespace CW88.TeleBot.ServiceBot;

public class UserStateManager(ICacheService cacheService) : IUserStateManager
{
    public async Task SetUserState(long chatId, IBotCommand command)
    {
            var key = $"Telegram:{chatId}";
            await cacheService.SetAsync(key, command);
        }

    public async Task<UserStateResult> TryGetUserState(long chatId)
    {
            var key = $"Telegram:{chatId}";
            var serializedCommand = await cacheService.GetAsync(key, typeof(IBotCommand));
            if (serializedCommand == null) return new UserStateResult { IsSuccess = false, Command = null };

            return new UserStateResult { IsSuccess = true, Command = (IBotCommand)serializedCommand };
        }

    public async Task RemoveUserState(long chatId)
    {
            var key = $"Telegram:{chatId}";
            await cacheService.RemoveAsync(key);
        }
}