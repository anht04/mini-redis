using Common.Helpers;
using MiniRedis.Models;

namespace MiniRedis.Commands
{
    public class LRangeCommand : ICommand
    {
        public int Arity => -4;

        public bool IsWriteCommand => false;

        public string Execute(List<string> args, Dictionary<RedisEntry, RedisValue> cache)
        {
            var cacheKey = args[1];
            var startIndex = int.Parse(args[2]);
            var endIndex = int.Parse(args[3]);

            cache.TryGetValue(new RedisEntry { Key = cacheKey }, out var value);

            if (value is null)
            {
                return RESPFormatHelper.FormatArray(value: null);
            }

            var parsedValue = value.AsList();

            if(startIndex >= parsedValue.Count)
            {
                return RESPFormatHelper.FormatArray(value: null);
            }

            if (endIndex >= parsedValue.Count)
            {
                endIndex = parsedValue.Count - 1;
            }

            return RESPFormatHelper.FormatArray(value.AsList().GetRange(startIndex, endIndex - startIndex + 1));
        }
    }
}
