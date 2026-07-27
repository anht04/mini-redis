using Common.Constants;
using MiniRedis.Models.GlobalCache;

namespace MiniRedis.Commands.Requests
{
    public record LRangeRequest
    {
        public RedisEntry Key { get; }
        public int FromIndex { get; }
        public int ToIndex { get; }

        private LRangeRequest(RedisEntry key, int fromIndex, int toIndex)
        {
            Key = key;
            FromIndex = fromIndex;
            ToIndex = toIndex;
        }

        public static LRangeRequest Create(List<string> args)
        {
            if (args.Count < 4 || !int.TryParse(args[2], out var fromIndex) || !int.TryParse(args[3], out var toIndex))
            {
                throw new InvalidOperationException(RedisErrorMessages.InvalidArgument);
            }

            return new LRangeRequest(new RedisEntry { Key = args[1] }, fromIndex, toIndex);
        }
    }
}
