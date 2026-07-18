namespace MiniRedis.Models;

public class RedisStream
{
    public Dictionary<string, List<RedisStreamData>> Data { get; set; } = [];

    public RedisStream(string key, RedisStreamData streamData)
    {
        Data[key] =  [streamData];
    }    
    
    public RedisStream(string key, List<RedisStreamData> streamDataList)
    {
        Data[key] =  streamDataList;
    }

    public void AddDataRange(string key, List<RedisStreamData> streamValues)
    {
        if (!Data.TryGetValue(key, out var value))
        {
            value = streamValues;
            Data[key] = value;
        }

        value.AddRange(streamValues);
    }
}