using Common.Helpers;
using MiniRedis.Models;

namespace MiniRedis.Commands
{
    public class LPushCommand : ICommand
    {
        public int Arity => -3;

        public bool IsWriteCommand => throw new NotImplementedException();

        public string Execute(List<string> args, Dictionary<RedisEntry, RedisValue> cache)
        {
            var cacheKey = new RedisEntry { Key = args[1] };
            var insertValues = args[2..];
            insertValues.Reverse();

            cache.TryGetValue(cacheKey, out var value);

            if (value == null)
            {
                cache.Add(cacheKey, new RedisValue(insertValues));
                return RESPFormatHelper.FormatInteger(insertValues.Count.ToString());
            }
            if (!value.IsList)
            {
                return RESPFormatHelper.FormatErrorString($"Duplicate key: {cacheKey}");
            }

            var parsedValue = value.AsList();
            parsedValue.InsertRange(0, insertValues);
            return RESPFormatHelper.FormatInteger(parsedValue.Count.ToString());
        }
    }
}
