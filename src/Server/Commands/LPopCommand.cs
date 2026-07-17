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
            var numberOfPopItem = 1;
            if (args.Count > 2)
            {
                numberOfPopItem = int.Parse(args[2]);
            }
            if (cache.TryGetValue(cacheKey, out var value) && !value.IsList)
            {
                return RESPFormatHelper.FormatErrorString("WRONGTYPE Operation against a key holding the wrong kind of value");
            }
            var parsedValue = value?.AsList();
            if (parsedValue is null || parsedValue.Count == 0)
            {
                return RedisConstants.NullBulkString;
            }
            var result = new List<string>();
            for (int i = 0; i < numberOfPopItem; i++)
            {
                result.Add(parsedValue[i]);
            }
            parsedValue.RemoveRange(0, numberOfPopItem);
            return result.Count > 1
                ? RESPFormatHelper.FormatArray(result)
                : RESPFormatHelper.FormatBulkString(result.First());
        }
    }
}
