namespace MiniRedis.Models.RedisStream;

public record RedisStreamDataValue
{
    public string Key { get; set; }
    public string Value { get; set; }

    private RedisStreamDataValue(string key, string value)
    {
        Key = key;
        Value = value;
    }

    public static RedisStreamDataValue Create(string key, string value)
    {
        return new RedisStreamDataValue(key, value);
    }
}