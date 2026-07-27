using Common.Constants;
using MiniRedis.Models.GlobalCache;

namespace MiniRedis.Commands.Requests
{
    public record LPopRequest
    {
        public RedisEntry Key { get; }
        public int Count { get; }

        private LPopRequest(RedisEntry key, int count)
        {
            Key = key;
            Count = count;
        }

        public static LPopRequest Create(List<string> args)
        {
            if (args.Count < 2)
            {
                throw new InvalidOperationException(RedisErrorMessages.InvalidArgument);
            }

            var hasCountArg = args.Count > 2;
            var count = 1;
            if (hasCountArg && !int.TryParse(args[2], out count))
            {
                throw new InvalidOperationException(RedisErrorMessages.InvalidArgument);
            }

            return new LPopRequest(new RedisEntry { Key = args[1] }, count);
        }
    }
}
