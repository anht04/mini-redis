using Common.Constants;

namespace MiniRedis.Models;

public class RedisStream
{
    public Dictionary<string, RedisStreamData> Data { get; set; } = [];

    public RedisStream(string key, RedisStreamData streamData)
    {
        Data[key] = streamData;
    }

    public void AddDataRange(string streamEntryKey, string dataId, List<RedisStreamDataValue> streamDataValues)
    {
        if (!Data.TryGetValue(streamEntryKey, out var value))
        {
            value = new RedisStreamData(dataId, streamDataValues);
            Data[streamEntryKey] = value;
            return;
        }

        value.AddRange(dataId, streamDataValues);
    }

    public string? GetLastestDataIdByStreamKey(string key)
    {
        if (!Data.TryGetValue(key, out var data))
        {
            throw new InvalidOperationException(RedisErrorMessages.XAddStreamDataIdNotGreaterThan0);
        }

        return data.GetLatestKey();
    }
}