using System.Text;
using System.Text.Json;
using CW88.TeleBot.ServiceBot.Models;

namespace CW88.TeleBot.ServiceBot.Utils;

public static class DataExtensions
{
    public static CallbackData? BuildCallbackData(string? callbackDataString)
    {
            try
            {
                if (string.IsNullOrWhiteSpace(callbackDataString))
                    return null;

                var callbackData = JsonSerializer.Deserialize<CallbackData>(callbackDataString);

                return callbackData;
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Error processing callback data: {ex.Message}\nCallback Data String: {callbackDataString}");
                return null;
            }
        }

    /// <summary>
    /// Serialize the CallbackData and ensure it's within 1-64 bytes when encoded in UTF-8.
    /// </summary>
    /// <param name="callbackDataObject"></param>
    /// <returns>Serialized string. Throws an exception if it's outside the byte range.</returns>
    public static string SerializeCallbackData(CallbackData callbackDataObject)
    {
            var (commandText, commandName) = callbackDataObject;
            var serializedString = $"{commandText}:{commandName}";

            if (!IsWithinByteRange(serializedString, 1, 64))
            {
                throw new InvalidOperationException("Serialized data is outside the allowed byte range.");
            }

            return serializedString;
        }

    private static bool IsWithinByteRange(string input, int min, int max)
    {
            var byteCount = Encoding.UTF8.GetByteCount(input);
            return byteCount >= min && byteCount <= max;
        }
}