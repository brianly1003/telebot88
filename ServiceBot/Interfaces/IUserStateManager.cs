using W88.TeleBot.ServiceBot.Models;

namespace W88.TeleBot.ServiceBot.Interfaces;

public interface IUserStateManager
{
    Task SetUserState(long chatId, IBotCommand botCommand);
    Task<UserStateResult> TryGetUserState(long chatId);
    Task RemoveUserState(long chatId);
}