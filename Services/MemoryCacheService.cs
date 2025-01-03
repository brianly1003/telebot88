using CW88.TeleBot.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace CW88.TeleBot.Services;

public class MemoryCacheService(IMemoryCache memoryCache) : ICacheService
{
    public Task<object> TryGetValueAsync(string key, Type t)
    {
        var found = memoryCache.TryGetValue(key, out var value);
        return Task.FromResult(value);
    }

    public async Task<object?> GetAsync(string key, Type type)
    {
        object? result = null;

        result = await TryGetValueAsync(key, type);

        return result;
    }

    public Task RemoveAsync(string key)
    {
        memoryCache.Remove(key);
        return Task.CompletedTask;
    }

    public Task SetAsync(string key, object value, TimeSpan? slidingExpireTime = null,
        DateTimeOffset? absoluteExpireTime = null)
    {
        var options = new MemoryCacheEntryOptions();

        if (absoluteExpireTime.HasValue)
        {
            options.AbsoluteExpiration = absoluteExpireTime;
        }
        else if (slidingExpireTime.HasValue)
        {
            options.SlidingExpiration = slidingExpireTime;
        }

        memoryCache.Set(key, value, options);

        return Task.CompletedTask;
    }
}