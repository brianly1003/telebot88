 using System.Collections.Concurrent;
using Telegram.Bot.Types;

namespace CW88.TeleBot.Services.Queues;

public class UpdateQueue(int maxProcessedIds = 10000)
{
    private readonly ConcurrentQueue<(string BotName, Update Update)> _updates = new();
    private readonly ConcurrentDictionary<long, byte> _processedUpdateIds = new();

    public void Enqueue(Update update, string botName)
    {
        if (_processedUpdateIds.ContainsKey(update.Id)) return;

        _updates.Enqueue((botName, update));

        _processedUpdateIds.TryAdd(update.Id, 0);

        // Limit memory growth by trimming old entries
        if (_processedUpdateIds.Count <= maxProcessedIds) return;

        var oldestKey = _processedUpdateIds.Keys.FirstOrDefault();
        _processedUpdateIds.TryRemove(oldestKey, out _);
    }

    public bool TryDequeue(out (string BotName, Update Update) result)
    {
        return _updates.TryDequeue(out result);
    }

    public async Task MarkAsProcessedAsync(long updateId)
    {
       
    }
}