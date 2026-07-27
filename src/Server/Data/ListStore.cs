using MiniRedis.Commands.AsyncManagers;
using MiniRedis.Models;
using MiniRedis.Models.GlobalCache;

namespace MiniRedis.Data
{
    public class ListStore
    {
        private readonly Dictionary<RedisEntry, RedisValue> _cache;

        public ListStore(Dictionary<RedisEntry, RedisValue> cache)
        {
            _cache = cache;
        }

        public int LLen(RedisEntry cacheKey)
        {
            if (!_cache.TryGetValue(cacheKey, out var redisValue))
            {
                return 0;
            }

            return redisValue.AsList().Count;
        }

        public List<string> LPop(RedisEntry cacheKey, int count)
        {
            if (!_cache.TryGetValue(cacheKey, out var redisValue))
            {
                return [];
            }

            var valueList = redisValue.AsList();
            if (valueList.Count == 0)
            {
                return [];
            }

            var actualPopCount = Math.Min(count, valueList.Count);
            var poppedItems = valueList.GetRange(0, actualPopCount);
            valueList.RemoveRange(0, actualPopCount);

            return poppedItems;
        }

        public int LPush(RedisEntry cacheKey, List<string> insertValues)
        {
            insertValues.Reverse();

            var waitingClient = BlockingManager.GetLongestClient(cacheKey.Key);
            if (waitingClient == null)
            {
                return LPushToCache(insertValues, cacheKey);
            }

            waitingClient.SubscribedTo.SetResult(insertValues[0]);
            if (insertValues.Count == 1)
            {
                return 1;
            }

            var remainingValues = insertValues.Skip(1).ToList();
            return LPushToCache(remainingValues, cacheKey, 1);
        }

        public int RPush(RedisEntry cacheKey, List<string> insertValues)
        {
            var waitingClient = BlockingManager.GetLongestClient(cacheKey.Key);
            if (waitingClient == null)
            {
                return RPushToCache(insertValues, cacheKey);
            }

            waitingClient.SubscribedTo.SetResult(insertValues[0]);
            if (insertValues.Count == 1)
            {
                return 1;
            }

            var remainingValues = insertValues.Skip(1).ToList();
            return RPushToCache(remainingValues, cacheKey, 1);
        }

        public List<string>? LRange(RedisEntry cacheKey, int fromIndex, int toIndex)
        {
            _cache.TryGetValue(cacheKey, out var value);

            if (value is null)
            {
                return null;
            }

            var parsedValue = value.AsList();
            var normalizedFromIndex = ConvertToPositiveIndex(parsedValue, rawIndex: fromIndex);
            var normalizedToIndex = ConvertToPositiveIndex(parsedValue, rawIndex: toIndex);

            if (normalizedFromIndex >= parsedValue.Count)
            {
                return null;
            }

            if (normalizedToIndex >= parsedValue.Count)
            {
                normalizedToIndex = parsedValue.Count - 1;
            }

            return parsedValue.GetRange(normalizedFromIndex, normalizedToIndex - normalizedFromIndex + 1);
        }

        public async Task<(string Key, string Item)?> BLPopAsync(RedisEntry cacheKey, SubscribedClient currentClient)
        {
            _cache.TryGetValue(cacheKey, out var redisValue);

            var valueList = redisValue?.AsList() ?? [];

            var poppedItem = PopFromList(valueList);
            if (poppedItem != null)
            {
                return (cacheKey.Key, poppedItem);
            }

            BlockingManager.Subscribe(cacheKey.Key, currentClient);

            var delayMilliseconds = currentClient.TimeoutInSeconds is > 0
                ? (int)(currentClient.TimeoutInSeconds.Value * 1000)
                : Timeout.Infinite;

            var timeoutDelayTask = Task.Delay(delayMilliseconds);
            var completedTask = await Task.WhenAny(currentClient.SubscribedTo.Task, timeoutDelayTask);

            if (completedTask == currentClient.SubscribedTo.Task)
            {
                var item = await currentClient.SubscribedTo.Task;
                return (cacheKey.Key, item);
            }

            return null;
        }

        private static string? PopFromList(List<string> list)
        {
            if (list.Count == 0)
            {
                return null;
            }

            var poppedItem = list[0];
            list.RemoveAt(0);
            return poppedItem;
        }

        private int LPushToCache(List<string> values, RedisEntry cacheKey, int valuesSentToClientCount = 0)
        {
            if (!_cache.TryGetValue(cacheKey, out var value))
            {
                _cache.Add(cacheKey, new RedisValue(values));
                return values.Count + valuesSentToClientCount;
            }

            var valueList = value.AsList();
            valueList.InsertRange(0, values);

            return valueList.Count + valuesSentToClientCount;
        }

        private int RPushToCache(List<string> values, RedisEntry cacheKey, int valuesSentToClientCount = 0)
        {
            if (!_cache.TryGetValue(cacheKey, out var value))
            {
                _cache.Add(cacheKey, new RedisValue(values));
                return values.Count + valuesSentToClientCount;
            }

            var valueList = value.AsList();
            valueList.AddRange(values);

            return valueList.Count + valuesSentToClientCount;
        }

        private static int ConvertToPositiveIndex(List<string> collection, int rawIndex)
        {
            if (rawIndex < 0)
            {
                var result = collection.Count + rawIndex;
                return result >= 0 ? result : 0;
            }

            return rawIndex;
        }
    }
}
