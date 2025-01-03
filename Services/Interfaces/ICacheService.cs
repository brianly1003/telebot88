namespace CW88.TeleBot.Services.Interfaces;

public interface ICacheService
{
    Task<object?> GetAsync(string key, Type t);

    public Task SetAsync(string key, object value, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null);

    public Task RemoveAsync(string key);
}