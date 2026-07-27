using Common.Constants;
using MiniRedis.Models.GlobalCache;

namespace MiniRedis.Commands.Requests
{
    public record SetRequest
    {
        public RedisEntry CacheKey { get; }
        public string Value { get; }

        private SetRequest(RedisEntry cacheKey, string value)
        {
            CacheKey = cacheKey;
            Value = value;
        }

        public static SetRequest Create(List<string> args)
        {
            if (args.Count < 3)
            {
                throw new InvalidOperationException(RedisErrorMessages.InvalidArgument);
            }

            long? expireAtMs = null;
            if (args.Count >= 5)
            {
                if (!int.TryParse(args[4], out var expireDuration))
                {
                    throw new InvalidOperationException(RedisErrorMessages.InvalidArgument);
                }

                expireAtMs = args[3].ToUpper() switch
                {
                    "PX" => DateTimeOffset.UtcNow.AddMilliseconds(expireDuration).ToUnixTimeMilliseconds(),
                    "EX" => DateTimeOffset.UtcNow.AddSeconds(expireDuration).ToUnixTimeMilliseconds(),
                    _ => null
                };
            }

            var cacheKey = new RedisEntry { Key = args[1], ExpireAtMs = expireAtMs };
            return new SetRequest(cacheKey, args[2]);
        }
    }
}
