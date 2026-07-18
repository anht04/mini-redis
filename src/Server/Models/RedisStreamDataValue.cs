namespace MiniRedis.Models;

public class RedisStreamDataValue
{
    public string Key { get; set; }
    public string Value { get; set; }

    public RedisStreamDataValue(string key, string value)
    {
        Key = key;
        Value = value;
    }
}