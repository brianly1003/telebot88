using System.Net;
using System.Security.Cryptography;
using System.Text;
using CW88.TeleBot.ServiceBot.Interfaces;
using CW88.TeleBot.ServiceBot.Models;
using Newtonsoft.Json;

namespace CW88.TeleBot.ServiceBot.Utils;

public class TelegramAuth(ILogger<TelegramAuth> logger) : ITelegramAuth
{
    public AuthData GetData(string authData)
    {
        var dataPairs = authData.Split('&')
            .Select(part => part.Split('='))
            .ToDictionary(split => split[0], split => WebUtility.UrlDecode(split[1])); // Decode URL-encoded strings

        var authDataObj = new AuthData
        {
            AuthDate = dataPairs["auth_date"],
            Hash = dataPairs["hash"],
            User = new User
            {
                Id = long.Parse(dataPairs["id"]),
                FirstName = dataPairs["first_name"],
                LastName = dataPairs.TryGetValue("last_name", out var lastName) ? lastName : null,
                Username = dataPairs.TryGetValue("username", out var username) ? username : null,
                PhotoUrl = dataPairs.TryGetValue("photo_url", out var photoUrl) ? photoUrl : null
            },
        };

        return authDataObj;
    }


    public AuthData GetMiniAppData(string authData)
    {
        var dataPairs = authData.Split('&').Select(part => part.Split('='))
            .ToDictionary(split => split[0], split => WebUtility.UrlDecode(split[1])); // Decode URL-encoded strings

        var authDataObj = new AuthData
        {
            QueryId = dataPairs.TryGetValue("query_id", out var queryId) ? queryId : null,
            AuthDate = dataPairs["auth_date"],
            Hash = dataPairs["hash"],
            User = JsonConvert.DeserializeObject<User>(WebUtility.UrlDecode(dataPairs["user"]))
        };

        return authDataObj;
    }

    public bool ValidateData(string authData, string botToken, string cStr = "", long timeValidInSeconds = 5 * 60)
    {
        try
        {
            // Parse the received data
            var dataPairs = authData.Split('&').Select(part => part.Split('='))
                .ToDictionary(split => split[0],
                    split => WebUtility.UrlDecode(split[1])); // Decode URL-encoded strings

            if (!dataPairs.ContainsKey("hash"))
            {
                return false;
            }

            // Extract auth_date from dataPairs
            if (!dataPairs.TryGetValue("auth_date", out var authDate))
            {
                return false;
            }

            if (!long.TryParse(authDate, out var receivedAuthDate))
            {
                return false; // Invalid auth_date format
            }

            // Check the time validity if timeValidInMilliseconds is not -1
            if (timeValidInSeconds != -1)
            {
                // Get current Unix timestamp
                var currentUnixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                // Check if data is outdated based on the specified timeValidInMilliseconds
                if (currentUnixTimestamp - receivedAuthDate > timeValidInSeconds)
                {
                    return false; // Data is outdated
                }
            }

            var receivedHash = dataPairs["hash"];

            // Remove hash to compute our own hash
            dataPairs.Remove("hash");

            // Create the data-check-string
            var dataCheckString = string.Join("\n", dataPairs.OrderBy(pair => pair.Key)
                .Select(pair => $"{pair.Key}={pair.Value}"));

            // Compute the secret key
            byte[] secretKey;
            if (string.IsNullOrEmpty(cStr))
            {
                using var sha256 = SHA256.Create();
                var botTokenBytes = Encoding.UTF8.GetBytes(botToken);
                secretKey = sha256.ComputeHash(botTokenBytes);
            }
            else
            {
                secretKey = ComputeHMACSHA256(Encoding.UTF8.GetBytes(cStr), Encoding.UTF8.GetBytes(botToken));
            }

            // Compute our hash
            var ourHashBytes = ComputeHMACSHA256(secretKey, Encoding.UTF8.GetBytes(dataCheckString));
            var ourHash = BitConverter.ToString(ourHashBytes).Replace("-", "").ToLower();

            // Compare the hashes
            return ourHash == receivedHash;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return false;
        }
    }

    public string EncryptData(string authData, string botToken, string cStr = "")
    {
        // Parse the received data
        var dataPairs = authData.Split('&').Select(part => part.Split('='))
            .ToDictionary(split => split[0], split => WebUtility.UrlDecode(split[1]));

        // Remove any existing hash from dataPairs (if present) to avoid conflicts
        dataPairs.Remove("hash");

        // Create the data-check-string (sorted key-value pairs joined with newline)
        var dataCheckString = string.Join("\n", dataPairs.OrderBy(pair => pair.Key)
            .Select(pair => $"{pair.Key}={pair.Value}"));

        // Compute the secret key using botToken or cStr
        byte[] secretKey;
        if (string.IsNullOrEmpty(cStr))
        {
            using var sha256 = SHA256.Create();
            var botTokenBytes = Encoding.UTF8.GetBytes(botToken);
            secretKey = sha256.ComputeHash(botTokenBytes);
        }
        else
        {
            secretKey = ComputeHMACSHA256(Encoding.UTF8.GetBytes(cStr), Encoding.UTF8.GetBytes(botToken));
        }

        // Encrypt the data-check string using the secret key
        var encryptedDataBytes = ComputeHMACSHA256(secretKey, Encoding.UTF8.GetBytes(dataCheckString));

        // Convert the encrypted data to a hex string for easier handling
        var encryptedData = BitConverter.ToString(encryptedDataBytes).Replace("-", "").ToLower();

        return encryptedData;
    }

    public string BuildQueryString(Dictionary<string, string?> parameters)
    {
        var builder = new StringBuilder();
        foreach (var param in parameters.Where(param => !string.IsNullOrEmpty(param.Value)))
        {
            if (builder.Length > 0)
                builder.Append("&");

            builder.Append($"{param.Key}={param.Value}");
        }
        return builder.ToString();
    }


    private static byte[] ComputeHMACSHA256(byte[] key, byte[] message)
    {
        using var hash = new HMACSHA256(key);
        return hash.ComputeHash(message);
    }
}