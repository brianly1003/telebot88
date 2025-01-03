using Newtonsoft.Json;

namespace CW88.TeleBot.ServiceBot.Models;

public class TokenRequest
{
    [JsonProperty("_auth")]
    public string AuthData { get; set; }
    public string GrantType { get; set; }
    public string RefreshToken { get; set; }
}