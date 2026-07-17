using Common.Constants;
using Common.Helpers;
using MiniRedis.Models;

namespace MiniRedis.Commands
{
    internal class LPopCommand : ICommand
    {
        public int Arity => -2;

        public bool IsWriteCommand => true;

        public string Execute(List<string> args, Dictionary<RedisEntry, RedisValue> cache)
        {
            var cacheKey = new RedisEntry { Key = args[1] };
            bool hasCountArg = args.Count > 2;
            int requestedPopCount = hasCountArg ? int.Parse(args[2]) : 1;

            if (!cache.TryGetValue(cacheKey, out var redisValue))
            {
                return hasCountArg ? RedisConstants.NullArray : RedisConstants.NullBulkString;
            }

            List<string>? valueList;
            try
            {
                valueList = redisValue.AsList();
            }
            catch (Exception)
            {
                return RESPFormatHelper.FormatErrorString(RedisErrorMessages.WrongTypeOperation);
            }

            if (valueList.Count == 0)
            {
                return hasCountArg ? RedisConstants.NullArray : RedisConstants.NullBulkString;
            }

            int actualPopCount = Math.Min(requestedPopCount, valueList.Count);
            var poppedItems = valueList.GetRange(0, actualPopCount);
            valueList.RemoveRange(0, actualPopCount);

            if (!hasCountArg)
            {
                return RESPFormatHelper.FormatBulkString(poppedItems[0]);
            }

            return RESPFormatHelper.FormatArray(poppedItems);
        }
    }
}
