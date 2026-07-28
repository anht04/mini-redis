using Common.Constants;
using MiniRedis.Constants;
using MiniRedis.Enums;

namespace MiniRedis.Models.RedisStream;

public class RedisStreamData
{
    private OrderedDictionary<RedisStreamDataId, List<RedisStreamDataValue>> Data { get; } = [];

    private RedisStreamData()
    {
    }

    public static RedisStreamData Empty() => new();

    public RedisStreamDataId AddRange(RedisStreamDataId baseNewDataId, List<RedisStreamDataValue> streamDataValues,
        StreamDataIdPattern idPattern)
    {
        if (idPattern == StreamDataIdPattern.FullForm)
        {
            if (baseNewDataId.Timestamp == 0 && baseNewDataId.Sequence == 0)
            {
                throw new InvalidOperationException(RedisErrorMessages.XAddStreamDataIdNotGreaterThan0);
            }

            if (!RedisStreamDataId.IsGreaterThan(baseNewDataId, GetCurrentLargestId()))
            {
                throw new InvalidOperationException(RedisErrorMessages.XAddStreamDataIdSmallerThanTopItem);
            }
        }

        var firstId = RedisStreamDataId.Create(baseNewDataId.Timestamp, baseNewDataId.Sequence);
        foreach (var streamDataValue in streamDataValues)
        {
            AddNewData(baseNewDataId, streamDataValue);
            baseNewDataId = RedisStreamDataId.GetNextId(baseNewDataId);
        }

        return firstId;
    }

    private RedisStreamDataId AddNewData(RedisStreamDataId dataId, RedisStreamDataValue streamValue)
    {
        if (Data.TryGetValue(dataId, out var streamDataValues))
        {
            throw new InvalidOperationException($"Stream Data key {dataId} already exists");
        }

        Data.Add(dataId, [streamValue]);
        return dataId;
    }

    public List<KeyValuePair<RedisStreamDataId, List<RedisStreamDataValue>>> GetRange(RedisStreamDataId? startId, RedisStreamDataId? endId)
    {
        var dataInRange = Data
            .Where(d =>
                (startId == null || (d.Key.Timestamp >= startId.Timestamp && d.Key.Sequence >= startId.Sequence)) &&
                (endId == null || (d.Key.Timestamp <= endId.Timestamp && d.Key.Sequence <= endId.Sequence)))
            .ToList();

        return dataInRange;
    }    
    
    public List<KeyValuePair<RedisStreamDataId, List<RedisStreamDataValue>>> GetRangeGreaterThan(RedisStreamDataId? startId)
    {
        var dataInRange = Data
            .Where(d => startId == null || (d.Key.Timestamp >= startId.Timestamp && d.Key.Sequence > startId.Sequence))
            .ToList();

        return dataInRange;
    }

    public KeyValuePair<RedisStreamDataId, List<RedisStreamDataValue>>? GetLast(RedisStreamDataId? id)
    {
        var collection = Data
            .Where(d => id == null || (d.Key.Timestamp >= id.Timestamp && d.Key.Sequence > id.Sequence));

        if(!collection.Any())
        {
            return null;
        }

        return collection.MaxBy(d => d.Key.Sequence);
    }

    public RedisStreamDataId? GetCurrentLargestId(long timestamp)
    {
        if (Data.Count == 0)
        {
            return null;
        }

        return Data
            .Where(d => d.Key.Timestamp == timestamp)
            .OrderByDescending(d => d.Key.Sequence)
            .FirstOrDefault()
            .Key;
    }

    public RedisStreamDataId GetNextId(long timeStamp, StreamDataIdPattern idPattern)
    {
        var matchingData = Data
            .Where(d => d.Key.Timestamp == timeStamp)
            .ToList();

        if (matchingData.Count == 0)
        {
            return RedisStreamDataId.Create(timeStamp, StreamConstants.DefaultSequenceNumberForNonZeroTimestamp);
        }

        return matchingData
            .OrderByDescending(d => d.Key.Timestamp)
            .ThenByDescending(d => d.Key.Sequence)
            .FirstOrDefault()
            .Key
            .GetNextId();
    }
    
    private RedisStreamDataId? GetCurrentLargestId()
    {
        if (Data.Count == 0)
        {
            return null;
        }

        return Data
            .OrderByDescending(d => d.Key.Timestamp)
            .ThenByDescending(d => d.Key.Sequence)
            .FirstOrDefault()
            .Key;
    }
}