using Newtonsoft.Json;

namespace W88.TeleBot.ServiceBot.Models;

public class ClientRequest
{
    [JsonProperty("_auth")]
    public string AuthData { get; set; }

    public string Method { get; set; }

    public string? Command { get; set; }
}