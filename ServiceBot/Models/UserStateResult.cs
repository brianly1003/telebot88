using CW88.TeleBot.ServiceBot.Interfaces;

namespace CW88.TeleBot.ServiceBot.Models;

public class UserStateResult
{
    public bool IsSuccess { get; set; }
    public IBotCommand? Command { get; set; }
}