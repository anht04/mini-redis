using Common.Constants;

namespace MiniRedis.Models;

public class RedisStreamData
{
    public Dictionary<string, List<RedisStreamDataValue>> Values { get; set; } = [];

    public RedisStreamData(string key, List<RedisStreamDataValue> streamValues)
    {
        Values[key] =  streamValues;
    }

    public void AddRange(string key, List<RedisStreamDataValue> streamValues)
    {
        if (!Values.TryGetValue(key, out var value))
        {
            value = streamValues;
            Values[key] = value;
        }

        value.AddRange(streamValues);
    }
    
    public static RedisStreamIdMetadata GetMetadataFromKey(string key)
    {
        if (!TryParseStreamDataId(key, out var time, out var sequence))
        {
            throw new InvalidOperationException(RedisErrorMessages.XAddStreamDataIdNotGreaterThan0);
        }

        return new RedisStreamIdMetadata(time, sequence);
    }
    
    public static bool TryParseStreamDataId(string input, out long time, out long sequence)
    {
        time = 0;
        sequence = 0;

        var parts = input.Split('-');
        if (parts.Length != 2)
        {
            return false;
        }

        if (!long.TryParse(parts[0], out time) || time < 0)
        {
            return false;
        }

        if (!long.TryParse(parts[1], out sequence) || sequence < 0)
        {
            return false;
        }

        return true;
    }

    public string? GetLatestKey()
    {
        if (Values.Count == 0)
        {
            return null;
        }
        return Values.MaxBy(v => v.Key).Key;
    }
}