using Newtonsoft.Json;
using TeleBot.ServiceBot.Models;

namespace TeleBot.ServiceBot.Utils
{
    public static class DataExtensions
    {
        public static CallbackData? BuildCallbackData(string callbackDataString)
        {
            try
            {
                return JsonConvert.DeserializeObject<CallbackData>(callbackDataString);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing JSON: {ex.Message}\nJSON String: {callbackDataString}");
                return null;
            }
        }
    }
}
