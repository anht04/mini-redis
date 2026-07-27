using Common.Constants;
using Common.Helpers;
using MiniRedis.Commands.AsyncManagers;
using MiniRedis.Commands.Requests;
using MiniRedis.Constants;
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

        public static string Echo(RedisEntry cacheKey)
        {
            return RESPFormatHelper.FormatBulkString(cacheKey.Key);
        }

        public string Get(RedisEntry cacheKey)
        {
            var targetKey = _cache.Keys.FirstOrDefault(k => k.Key == cacheKey.Key);
            if (targetKey == null)
            {
                return RedisConstants.NullBulkString;
            }

            if (targetKey.IsExpired)
            {
                _cache.Remove(targetKey);
                return RedisConstants.NullBulkString;
            }

            return RESPFormatHelper.FormatBulkString(_cache[targetKey].AsString());
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
                return RESPFormatHelper.FormatSimpleString(RedisErrorMessages.EnumValueNotFound);
            }

            var typeString = value.DataType switch
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

            return RESPFormatHelper.FormatSimpleString(typeString);
        }

        public string LLen(RedisEntry cacheKey)
        {
            if (!_cache.TryGetValue(cacheKey, out var redisValue))
            {
                return RESPFormatHelper.FormatInteger("0");
            }

            var itemCount = redisValue.AsList().Count;
            return RESPFormatHelper.FormatInteger(itemCount.ToString());
        }

        public string LPop(RedisEntry cacheKey, int count)
        {
            if (!_cache.TryGetValue(cacheKey, out var redisValue))
            {
                return count > 1 ? RedisConstants.NullArray : RedisConstants.NullBulkString;
            }

            var valueList = redisValue.AsList();
            if (valueList.Count == 0)
            {
                return count > 1 ? RedisConstants.NullArray : RedisConstants.NullBulkString;
            }

            int actualPopCount = Math.Min(count, valueList.Count);
            var poppedItems = valueList.GetRange(0, actualPopCount);
            valueList.RemoveRange(0, actualPopCount);

            return count > 1
                ? RESPFormatHelper.FormatArray(poppedItems)
                : RESPFormatHelper.FormatBulkString(poppedItems[0]);
        }

        public string LPush(RedisEntry cacheKey, List<string> insertValues)
        {
            insertValues.Reverse();

            var waitingClient = BlockingManager.GetLongestClient(cacheKey.Key);
            if (waitingClient == null)
            {
                return LPushToCacheAndFormat(insertValues, cacheKey);
            }

            waitingClient.SubscribedTo.SetResult(insertValues[0]);
            if (insertValues.Count == 1)
            {
                return RESPFormatHelper.FormatInteger(1);
            }

            var remainingValues = insertValues.Skip(1).ToList();
            return LPushToCacheAndFormat(remainingValues, cacheKey, 1);
        }

        public string RPush(RedisEntry cacheKey, List<string> insertValues)
        {
            var waitingClient = BlockingManager.GetLongestClient(cacheKey.Key);
            if (waitingClient == null)
            {
                return RPushToCacheAndFormat(insertValues, cacheKey);
            }

            waitingClient.SubscribedTo.SetResult(insertValues[0]);
            if (insertValues.Count == 1)
            {
                return RESPFormatHelper.FormatInteger(1);
            }

            var remainingValues = insertValues.Skip(1).ToList();
            return RPushToCacheAndFormat(remainingValues, cacheKey, 1);
        }

        public string LRange(RedisEntry cacheKey, int fromIndex, int toIndex)
        {
            _cache.TryGetValue(cacheKey, out var value);

            if (value is null)
            {
                return RESPFormatHelper.FormatArray(value: null);
            }

            var parsedValue = value.AsList();
            var normalizedFromIndex = ConvertToPositiveIndex(parsedValue, rawIndex: fromIndex);
            var normalizedToIndex = ConvertToPositiveIndex(parsedValue, rawIndex: toIndex);

            if (normalizedFromIndex >= parsedValue.Count)
            {
                return RESPFormatHelper.FormatArray(value: null);
            }

            if (normalizedToIndex >= parsedValue.Count)
            {
                normalizedToIndex = parsedValue.Count - 1;
            }

            return RESPFormatHelper.FormatArray(parsedValue.GetRange(normalizedFromIndex,
                normalizedToIndex - normalizedFromIndex + 1));
        }

        public async Task<string> BLPopAsync(RedisEntry cacheKey, SubscribedClient currentClient)
        {
            _cache.TryGetValue(cacheKey, out var redisValue);

            var valueList = redisValue?.AsList() ?? [];

            var poppedItem = PopFromList(valueList);
            if (poppedItem != null)
            {
                return RESPFormatHelper.FormatArray([cacheKey.Key, poppedItem]);
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
                return RESPFormatHelper.FormatArray([cacheKey.Key, item]);
            }

            return RedisConstants.NullArray;
        }

        public string XAdd(RedisEntry streamEntryKey, string streamDataId, List<RedisStreamDataValue> parsedStreamDataValues)
        {
            if (!_cache.TryGetValue(streamEntryKey, out var value))
            {
                var dataId = AddDataRangeToCache(streamEntryKey, streamDataId, parsedStreamDataValues);
                return RESPFormatHelper.FormatBulkString(dataId.ToString());
            }

            var addedDataId = AddDataRangeToCache(streamEntryKey, streamDataId, parsedStreamDataValues);

            return RESPFormatHelper.FormatBulkString(addedDataId.ToString());
        }

        public string XRange(RedisEntry cacheKey, string startId, string endId, XRangeCommandPurpose purpose)
        {
            if (!_cache.TryGetValue(cacheKey, out var value))
            {
                return RESPFormatHelper.FormatArray((string?)null);
            }

            var startPattern = StreamDataIdPattern.FullForm;
            long? startTimestamp = null;
            long? startSequenceNumber = null;
            var endPattern = StreamDataIdPattern.FullForm;
            long? endTimestamp = null;
            long? endSequenceNumber = null;
            if ((purpose != XRangeCommandPurpose.QueryWithStartArgument &&
                 !RedisStreamDataId.TryParseXRangeStreamDataId(startId, out startTimestamp, out startSequenceNumber, out startPattern)) ||
                (purpose != XRangeCommandPurpose.QueryWithEndArgument &&
                 !RedisStreamDataId.TryParseXRangeStreamDataId(endId, out endTimestamp, out endSequenceNumber, out endPattern)))
            {
                throw new InvalidOperationException(RedisErrorMessages.XAddStreamDataIdInvalidFormat);
            }

            if ((purpose != XRangeCommandPurpose.QueryWithStartArgument && startPattern != StreamDataIdPattern.FullForm &&
                 startPattern != StreamDataIdPattern.AutoGeneratedSequence) ||
                (purpose != XRangeCommandPurpose.QueryWithEndArgument && endPattern != StreamDataIdPattern.FullForm && endPattern != StreamDataIdPattern.AutoGeneratedSequence))
            {
                throw new InvalidOperationException(RedisErrorMessages.XAddStreamDataIdInvalidFormat);
            }

            var streamData = value.AsStream();
            if (startPattern == StreamDataIdPattern.AutoGeneratedSequence)
            {
                startSequenceNumber = 0;
            }

            if (endPattern == StreamDataIdPattern.AutoGeneratedSequence)
            {
                endSequenceNumber = streamData.GetCurrentLargestId(endTimestamp!.Value)?.Sequence ?? 0;
            }

            RedisStreamDataId? parsedStartId = null;
            if (purpose != XRangeCommandPurpose.QueryWithStartArgument)
            {
                parsedStartId = RedisStreamDataId.Create(startTimestamp!.Value, startSequenceNumber.Value);
            }

            RedisStreamDataId? parsedEndId = null;
            if (purpose != XRangeCommandPurpose.QueryWithEndArgument)
            {
                parsedEndId = RedisStreamDataId.Create(endTimestamp!.Value, endSequenceNumber!.Value);
            }

            var result = streamData
                .GetRange(parsedStartId, parsedEndId)
                .Select(kp =>
                    new KeyValuePair<string, List<string>>(
                        kp.Key.ToString(),
                        kp.Value.SelectMany(v => v.ToKeyValueStringArray()).ToList()))
                .ToList();

            return RESPFormatHelper.FormatArray(result);
        }

        public string XRead(XReadRequest request)
        {
            List<(string, List<KeyValuePair<string, List<string>>>?)>? results = [];
            foreach (var query in request.Queries)
            {
                var cacheKey = query.StreamId;
                var startId = query.DataId;

                if (!_cache.TryGetValue(cacheKey, out var value))
                {
                    results.Add((cacheKey.Key, null));
                    continue;
                }

                var streamData = value.AsStream();

                List<KeyValuePair<string, List<string>>>? result = streamData
                    .GetRangeGreaterThan(startId)
                    .Select(kp =>
                        new KeyValuePair<string, List<string>>(
                            kp.Key.ToString(),
                            kp.Value.SelectMany(v => v.ToKeyValueStringArray()).ToList()))
                    .ToList();

                results.Add((cacheKey.Key, result));
            }

            return RESPFormatHelper.FormatArray(results);
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

        private string LPushToCacheAndFormat(List<string> values, RedisEntry cacheKey, int valuesSentToClientCount = 0)
        {
            if (!_cache.TryGetValue(cacheKey, out var value))
            {
                _cache.Add(cacheKey, new RedisValue(values));
                return RESPFormatHelper.FormatInteger(values.Count + valuesSentToClientCount);
            }

            List<string> valueList = value.AsList();
            valueList.InsertRange(0, values);

            return RESPFormatHelper.FormatInteger(valueList.Count + valuesSentToClientCount);
        }

        private string RPushToCacheAndFormat(List<string> values, RedisEntry cacheKey, int valuesSentToClientCount = 0)
        {
            if (!_cache.TryGetValue(cacheKey, out var value))
            {
                _cache.Add(cacheKey, new RedisValue(values));
                return RESPFormatHelper.FormatInteger(values.Count + valuesSentToClientCount);
            }

            var valueList = value.AsList();
            valueList.AddRange(values);

            return RESPFormatHelper.FormatInteger(valueList.Count + valuesSentToClientCount);
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

        private RedisStreamDataId AddDataRangeToCache(RedisEntry streamId, string dataId,
            List<RedisStreamDataValue> streamDataValues)
        {
            var newDataId = GenerateNewDataId(streamId.Key, dataId, out var idGenerationBehaviour);

            if (!_cache.TryGetValue(streamId, out var streamData))
            {
                streamData = new RedisValue(RedisStreamData.Empty());
                _cache.Add(streamId, streamData);
            }

            var parsedStreamData = streamData.AsStream();
            return parsedStreamData.AddRange(newDataId, streamDataValues, idGenerationBehaviour);
        }

        private RedisStreamDataId GenerateNewDataId(string streamId, string dataId,
            out StreamDataIdPattern pattern)
        {
            var cacheKey = new RedisEntry { Key = streamId };
            if (!RedisStreamDataId.TryParseStreamDataId(dataId, out var timestamp, out var sequence,
                    out var dataIdGenerationBehaviour))
            {
                throw new InvalidOperationException(RedisErrorMessages.XAddStreamDataIdNotGreaterThan0);
            }

            pattern = dataIdGenerationBehaviour;

            var hasExistingStream = _cache.TryGetValue(cacheKey, out var streamData);
            var defaultSequenceForAutogeneratedSequence = timestamp == 0
                ? StreamConstants.DefaultSequenceNumberForZeroTimestamp
                : StreamConstants.DefaultSequenceNumberForNonZeroTimestamp;

            return dataIdGenerationBehaviour switch
            {
                StreamDataIdPattern.FullyAuto => RedisStreamDataId.Create(
                    DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), StreamConstants.DefaultSequenceNumberForNonZeroTimestamp),
                StreamDataIdPattern.AutoGeneratedSequence => hasExistingStream
                    ? streamData!.AsStream().GetNextId(timestamp!.Value, dataIdGenerationBehaviour)
                    : RedisStreamDataId.Create(timestamp!.Value, defaultSequenceForAutogeneratedSequence),
                StreamDataIdPattern.FullForm => RedisStreamDataId.Create(timestamp!.Value, sequence!.Value),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}