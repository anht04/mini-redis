using Common.Constants;
using MiniRedis.Models.GlobalCache;

namespace MiniRedis.Commands.Requests
{
    public record LPopRequest
    {
        public RedisEntry Key { get; }
        public int Count { get; }
        public bool HasExplicitCount { get; }

        private LPopRequest(RedisEntry key, int count, bool hasExplicitCount)
        {
            Key = key;
            Count = count;
            HasExplicitCount = hasExplicitCount;
        }

        public static LPopRequest Create(List<string> args)
        {
            if (args.Count < 2)
            {
                throw new InvalidOperationException(RedisErrorMessages.InvalidArgument);
            }

            var hasExplicitCount = args.Count > 2;
            var count = 1;
            if (hasExplicitCount && !int.TryParse(args[2], out count))
            {
                throw new InvalidOperationException(RedisErrorMessages.InvalidArgument);
            }

            return new LPopRequest(new RedisEntry { Key = args[1] }, count, hasExplicitCount);
        }
    }
}
