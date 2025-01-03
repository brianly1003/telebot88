using CI24.Apps.NetCore.HttpApiFacade.Core;
using Newtonsoft.Json;

namespace CW88.TeleBot.Services;

public class DefaultJsonSerializationService : IJsonSerializationService
{
    public object DeserializeObject(string jsonString)
        => JsonConvert.DeserializeObject(jsonString);

    public TObjectType DeserializeObject<TObjectType>(string jsonString)
        => JsonConvert.DeserializeObject<TObjectType>(jsonString);

    public string SerializeObject(object obj)
        => JsonConvert.SerializeObject(obj);
}