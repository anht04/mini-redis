using Common.Constants;
using MiniRedis.Models.GlobalCache;

namespace MiniRedis.Commands.Requests
{
    public record SingleKeyRequest
    {
        public RedisEntry Key { get; }

        private SingleKeyRequest(RedisEntry key)
        {
            Key = key;
        }

        public static SingleKeyRequest Create(List<string> args)
        {
            if (args.Count < 2)
            {
                throw new InvalidOperationException(RedisErrorMessages.InvalidArgument);
            }

            return new SingleKeyRequest(new RedisEntry { Key = args[1] });
        }
    }
}
