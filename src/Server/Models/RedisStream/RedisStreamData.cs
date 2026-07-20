using MiniRedis.Constants;
using MiniRedis.Enums;

namespace MiniRedis.Models.RedisStream;

public class RedisStreamData
{
    private Dictionary<RedisStreamDataId, List<RedisStreamDataValue>> Data { get; } = [];

    private RedisStreamData()
    {
    }

    public static RedisStreamData Create(RedisStreamDataId baseNewDataId, List<RedisStreamDataValue> streamDataValues,
        StreamDataIdGenerationBehaviour idGenerationBehaviour)
    {
        var streamData = new RedisStreamData();
        foreach (var streamDataValue in streamDataValues)
        {
            var newDataId = streamData.AddNewData(baseNewDataId, streamDataValue);
            baseNewDataId = RedisStreamDataId.GetNextId(newDataId, idGenerationBehaviour);
        }

        return streamData;
    }
    
    public RedisStreamDataId AddRange(RedisStreamDataId baseNewDataId, List<RedisStreamDataValue> streamDataValues, StreamDataIdGenerationBehaviour idGenerationBehaviour)
    {
        if (Data.TryGetValue(baseNewDataId, out var existingDataValues))
        {
            throw new InvalidOperationException($"Stream Data key {baseNewDataId} already exists");
        }

        RedisStreamDataId latestId = baseNewDataId;
        foreach (var streamDataValue in streamDataValues)
        {
            latestId = AddNewData(baseNewDataId, streamDataValue);
            baseNewDataId = RedisStreamDataId.GetNextId(latestId, idGenerationBehaviour);
        }

        return latestId;
    }
    
    public RedisStreamDataId AddNewData(RedisStreamDataId dataId, RedisStreamDataValue streamValue)
    {
        if (Data.TryGetValue(dataId, out var streamDataValues))
        {
            throw new InvalidOperationException($"Stream Data key {dataId} already exists");
        }

        Data.Add(dataId, [streamValue]);
        return dataId;
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

    public RedisStreamDataId GetNextId(long timeStamp, StreamDataIdGenerationBehaviour idGenerationBehaviour)
    {
        var matchingData = Data
            .Where(d => d.Key.Timestamp == timeStamp)
            .ToList();

        if (matchingData.Count == 0)
        {
            return RedisStreamDataId.Create(timeStamp, StreamConstants.DefaultSequenceNumberForNonZeroTimestamp,
                idGenerationBehaviour);
        }

        return matchingData
            .OrderByDescending(d => d.Key.Timestamp)
            .ThenByDescending(d => d.Key.Sequence)
            .FirstOrDefault()
            .Key
            .GetNextId(idGenerationBehaviour);
    }
}