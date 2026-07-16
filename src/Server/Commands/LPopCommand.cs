using Common;
using Common.Helpers;
using MiniRedis.Models;

namespace MiniRedis.Commands
{
    internal class LPopCommand : ICommand
    {
        public int Arity => -2;

        public bool IsWriteCommand => throw new NotImplementedException();

        public string Execute(List<string> args, Dictionary<RedisEntry, RedisValue> cache)
        {
            var cacheKey = new RedisEntry { Key = args[1] };
            if (cache.TryGetValue(cacheKey, out var value) && !value.IsList)
            {
                return RESPFormatHelper.FormatErrorString("WRONGTYPE Operation against a key holding the wrong kind of value");
            }
            var parsedValue = value?.AsList();
            if(parsedValue is null || parsedValue.Count == 0)
            {
                return RedisConstants.NullBulkString;
            }
            var firstValue = parsedValue[0];
            parsedValue.RemoveAt(0);
            return RESPFormatHelper.FormatBulkString(firstValue);
        }
    }
}
