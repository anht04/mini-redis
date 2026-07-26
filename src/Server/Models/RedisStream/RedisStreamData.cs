using MiniRedis.Constants;
using MiniRedis.Enums;

namespace MiniRedis.Models.RedisStream;

public class RedisStreamData
{
    private OrderedDictionary<RedisStreamDataId, List<RedisStreamDataValue>> Data { get; } = [];

    private RedisStreamData()
    {
    }

    public static RedisStreamData Create(RedisStreamDataId baseNewDataId, List<RedisStreamDataValue> streamDataValues,
        StreamDataIdPattern idPattern)
    {
        var streamData = new RedisStreamData();
        foreach (var streamDataValue in streamDataValues)
        {
            var newDataId = streamData.AddNewData(baseNewDataId, streamDataValue);
            baseNewDataId = RedisStreamDataId.GetNextId(newDataId, idPattern);
        }

        return streamData;
    }

    public RedisStreamDataId AddRange(RedisStreamDataId baseNewDataId, List<RedisStreamDataValue> streamDataValues,
        StreamDataIdPattern idPattern)
    {
        if (Data.TryGetValue(baseNewDataId, out var existingDataValues))
        {
            throw new InvalidOperationException($"Stream Data key {baseNewDataId} already exists");
        }

        var firstId = RedisStreamDataId.Create(baseNewDataId.Timestamp, baseNewDataId.Sequence);
        foreach (var streamDataValue in streamDataValues)
        {
            AddNewData(baseNewDataId, streamDataValue);
            baseNewDataId = RedisStreamDataId.GetNextId(baseNewDataId, idPattern);
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

    public RedisStreamDataId? GetCurrentSmallestId()
    {
        if (Data.Count == 0)
        {
            return null;
        }

        return Data
            .OrderBy(d => d.Key.Timestamp)
            .ThenBy(d => d.Key.Sequence)
            .FirstOrDefault()
            .Key;
    }

    public RedisStreamDataId? GetCurrentLargestId()
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
            return RedisStreamDataId.Create(timeStamp, StreamConstants.DefaultSequenceNumberForNonZeroTimestamp,
                idPattern);
        }

        return matchingData
            .OrderByDescending(d => d.Key.Timestamp)
            .ThenByDescending(d => d.Key.Sequence)
            .FirstOrDefault()
            .Key
            .GetNextId(idPattern);
    }
}