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

            var count = LPushToCache(insertValues, cacheKey);

            BlockingManager.SignalLongestClient(cacheKey.Key);

            return count;
        }

        public int RPush(RedisEntry cacheKey, List<string> insertValues)
        {
            var count = RPushToCache(insertValues, cacheKey);

            BlockingManager.SignalLongestClient(cacheKey.Key);

            return count;
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

        public string? BLPop(RedisEntry cacheKey)
        {
            if (!_cache.TryGetValue(cacheKey, out var redisValue))
            {
                return null;
            }

            var valueList = redisValue.AsList() ?? [];
            return PopFromList(valueList);
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

        private int LPushToCache(List<string> values, RedisEntry cacheKey)
        {
            if (!_cache.TryGetValue(cacheKey, out var value))
            {
                _cache.Add(cacheKey, new RedisValue(values));
                return values.Count;
            }

            var valueList = value.AsList();
            valueList.InsertRange(0, values);

            return valueList.Count;
        }

        private int RPushToCache(List<string> values, RedisEntry cacheKey)
        {
            if (!_cache.TryGetValue(cacheKey, out var value))
            {
                _cache.Add(cacheKey, new RedisValue(values));
                return values.Count;
            }

            var valueList = value.AsList();
            valueList.AddRange(values);

            return valueList.Count;
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
