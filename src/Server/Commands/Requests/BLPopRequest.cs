using Common.Constants;
using MiniRedis.Models.GlobalCache;

namespace MiniRedis.Commands.Requests
{
    public record BLPopRequest
    {
        public RedisEntry Key { get; }
        public float TimeoutInSeconds { get; }

        private BLPopRequest(RedisEntry key, float timeoutInSeconds)
        {
            Key = key;
            TimeoutInSeconds = timeoutInSeconds;
        }

        public static BLPopRequest Create(List<string> args)
        {
            if (args.Count < 3)
            {
                throw new InvalidOperationException(RedisErrorMessages.InvalidArgument);
            }

            if (!float.TryParse(args[2], out var timeoutInSeconds) || timeoutInSeconds < 0)
            {
                throw new InvalidOperationException(RedisErrorMessages.BLPopInvalidTimeout);
            }

            return new BLPopRequest(new RedisEntry { Key = args[1] }, timeoutInSeconds);
        }
    }
}
