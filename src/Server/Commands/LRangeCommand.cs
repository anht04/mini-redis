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

            cache.TryGetValue(new RedisEntry { Key = cacheKey }, out var value);

            if (value is null)
            {
                return RESPFormatHelper.FormatArray(value: null);
            }

            var parsedValue = value.AsList();
            var normalizedStartIndex = ConvertToPositiveIndex(parsedValue, rawIndex: int.Parse(args[2]));
            var normalizedEndIndex = ConvertToPositiveIndex(parsedValue, rawIndex: int.Parse(args[3]));

            if (normalizedStartIndex >= parsedValue.Count)
            {
                return RESPFormatHelper.FormatArray(value: null);
            }

            if (normalizedEndIndex >= parsedValue.Count)
            {
                normalizedEndIndex = parsedValue.Count - 1;
            }

            return RESPFormatHelper.FormatArray(parsedValue.GetRange(normalizedStartIndex, normalizedEndIndex - normalizedStartIndex + 1));
        }

        private static int ConvertToPositiveIndex(List<string> collection, int rawIndex)
        {
            if (rawIndex < 0)
            {
                var result = collection.Count + rawIndex;
                return result >= 0 ? result : 0;
            }
            return rawIndex;
        }
    }
}
