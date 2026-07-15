namespace MiniRedis.Models;

public record CacheEntry
{
    public string Key { get; init; }
    public long? ExpireAtMs { get; init; }
    public bool IsExpired => ExpireAtMs.HasValue && DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() >= ExpireAtMs.Value;
}