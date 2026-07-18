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
}