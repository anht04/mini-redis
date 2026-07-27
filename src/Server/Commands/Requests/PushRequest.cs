using Common.Constants;
using MiniRedis.Models.GlobalCache;

namespace MiniRedis.Commands.Requests
{
    public record PushRequest
    {
        public RedisEntry Key { get; }
        public List<string> Values { get; }

        private PushRequest(RedisEntry key, List<string> values)
        {
            Key = key;
            Values = values;
        }

        public static PushRequest Create(List<string> args)
        {
            if (args.Count < 3)
            {
                throw new InvalidOperationException(RedisErrorMessages.InvalidArgument);
            }

            return new PushRequest(new RedisEntry { Key = args[1] }, args[2..]);
        }
    }
}
