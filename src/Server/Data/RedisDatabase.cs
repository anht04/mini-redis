using Common.Constants;
using Common.Results;
using MiniRedis.Commands.Requests;
using MiniRedis.Enums;
using MiniRedis.Extensions;
using MiniRedis.Models;
using MiniRedis.Models.GlobalCache;
using MiniRedis.Models.RedisStream;

namespace MiniRedis.Data
{
    public class RedisDatabase
    {
        private readonly Dictionary<RedisEntry, RedisValue> _cache = [];
        private readonly ListStore _listStore;
        private readonly StreamStore _streamStore;

        public RedisDatabase()
        {
            _listStore = new ListStore(_cache);
            _streamStore = new StreamStore(_cache);
        }

        public static string Echo(RedisEntry cacheKey)
        {
            return cacheKey.Key;
        }

        public string? Get(RedisEntry cacheKey)
        {
            var targetKey = _cache.Keys.FirstOrDefault(k => k.Key == cacheKey.Key);
            if (targetKey == null)
            {
                return null;
            }

            if (targetKey.IsExpired)
            {
                _cache.Remove(targetKey);
                return null;
            }

            return _cache[targetKey].AsString();
        }

        public bool Set(RedisEntry cacheKey, string value)
        {
            var redisValue = new RedisValue(value);
            _cache[cacheKey] = redisValue;

            return true;
        }

        public string Type(RedisEntry cacheKey)
        {
            _cache.TryGetValue(cacheKey, out var value);

            if (value is null)
            {
                return RedisErrorMessages.EnumValueNotFound;
            }

            return value.DataType switch
            {
                RedisDataType.String => RedisDataType.String.GetDescription(),
                RedisDataType.List => RedisDataType.List.GetDescription(),
                RedisDataType.Set => RedisDataType.Set.GetDescription(),
                RedisDataType.ZSet => RedisDataType.ZSet.GetDescription(),
                RedisDataType.Hash => RedisDataType.Hash.GetDescription(),
                RedisDataType.Stream => RedisDataType.Stream.GetDescription(),
                RedisDataType.VectorSet => RedisDataType.VectorSet.GetDescription(),
                _ => RedisErrorMessages.EnumValueNotFound
            };
        }

        public int LLen(RedisEntry cacheKey) => _listStore.LLen(cacheKey);

        public List<string> LPop(RedisEntry cacheKey, int count) => _listStore.LPop(cacheKey, count);

        public int LPush(RedisEntry cacheKey, List<string> insertValues) => _listStore.LPush(cacheKey, insertValues);

        public int RPush(RedisEntry cacheKey, List<string> insertValues) => _listStore.RPush(cacheKey, insertValues);

        public List<string>? LRange(RedisEntry cacheKey, int fromIndex, int toIndex) =>
            _listStore.LRange(cacheKey, fromIndex, toIndex);

        public Task<(string Key, string Item)?> BLPopAsync(RedisEntry cacheKey, SubscribedClient currentClient) =>
            _listStore.BLPopAsync(cacheKey, currentClient);

        public RedisStreamDataId XAdd(RedisEntry streamEntryKey, string streamDataId, List<RedisStreamDataValue> parsedStreamDataValues) =>
            _streamStore.XAdd(streamEntryKey, streamDataId, parsedStreamDataValues);

        public List<StreamDataResult>? XRange(RedisEntry cacheKey, string startId, string endId, XRangeCommandPurpose purpose) =>
            _streamStore.XRange(cacheKey, startId, endId, purpose);

        public List<XReadStreamResult> XRead(XReadRequest request) =>
            _streamStore.XRead(request);
    }
}
