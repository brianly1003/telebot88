using CW88.TeleBot.ServiceBot.Models;

namespace CW88.TeleBot.ServiceBot.Interfaces;

public interface ITelegramAuth
{
    AuthData GetData(string authData);

    AuthData GetMiniAppData(string authData);

    bool ValidateData(string authData, string botToken, string cStr = "", long timeValidInSeconds = 5 * 60);

    string EncryptData(string authData, string botToken, string cStr = "");

    string BuildQueryString(Dictionary<string, string?> parameters);
}