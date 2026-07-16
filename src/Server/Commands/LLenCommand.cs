using Common.Helpers;
using MiniRedis.Models;

namespace MiniRedis.Commands
{
    public class LLenCommand : ICommand
    {
        public int Arity => -2;

        public bool IsWriteCommand => false;

        public string Execute(List<string> args, Dictionary<RedisEntry, RedisValue> cache)
        {
            var cacheKey = new RedisEntry { Key = args[1] };
            if (cache.TryGetValue(cacheKey, out var value) && !value.IsList)
            {
                return RESPFormatHelper.FormatErrorString("WRONGTYPE Operation against a key holding the wrong kind of value");
            }
            var itemCount = value?.AsList().Count ?? 0;
            return RESPFormatHelper.FormatInteger(itemCount.ToString());
        }
    }
}
