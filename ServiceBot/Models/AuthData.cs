using Newtonsoft.Json;

namespace W88.TeleBot.ServiceBot.Models;

public class AuthData
{
    [JsonProperty("query_id")]
    public string? QueryId { get; set; }

    public User? User { get; set; }

    [JsonProperty("auth_date")]
    public string AuthDate { get; set; }

    public string Hash { get; set; }
}

public class User
{
    public long Id { get; set; }

    [JsonProperty("first_name")]
    public string FirstName {get; set; }

    [JsonProperty("last_name")]
    public string? LastName { get; set; }

    public string? Username { get; set; }

    [JsonProperty("photo_url")]
    public string? PhotoUrl { get; set; }

    [JsonProperty("language_code")]
    public string? LanguageCode {get; set; }

    [JsonProperty("allow_write_to_pm")]
    public bool? AllowWriteToPm { get; set; }
}