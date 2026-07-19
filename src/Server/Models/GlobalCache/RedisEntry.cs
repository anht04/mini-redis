namespace MiniRedis.Models.GlobalCache;

public record RedisEntry
{
    public string Key { get; init; }
    public long? ExpireAtMs { get; init; }
    public bool IsExpired => ExpireAtMs.HasValue && DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() >= ExpireAtMs.Value;
}