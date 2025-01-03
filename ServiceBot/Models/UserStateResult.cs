using W88.TeleBot.ServiceBot.Interfaces;

namespace W88.TeleBot.ServiceBot.Models;

public class UserStateResult
{
    public bool IsSuccess { get; set; }
    public IBotCommand? Command { get; set; }
}