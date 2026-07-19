using System.Net.Sockets;
using Common.Constants;
using Common.Helpers;
using MiniRedis.Models.GlobalCache;

namespace MiniRedis.Commands
{
    internal class LPopCommand : ICommand
    {
        public int Arity => -2;

        public bool IsWriteCommand => true;

        public Task<string> ExecuteAsync(List<string> args, Dictionary<RedisEntry, RedisValue> cache, Socket client)
        {
            var cacheKey = new RedisEntry { Key = args[1] };
            bool hasCountArg = args.Count > 2;
            int requestedPopCount = hasCountArg ? int.Parse(args[2]) : 1;

            if (!cache.TryGetValue(cacheKey, out var redisValue))
            {
                return Task.FromResult(hasCountArg ? RedisConstants.NullArray : RedisConstants.NullBulkString);
            }

            List<string>? valueList;
            try
            {
                valueList = redisValue.AsList();
            }
            catch (Exception)
            {
                return Task.FromResult(RESPFormatHelper.FormatSimpleErrorString(RedisErrorMessages.WrongTypeOperation));
            }

            if (valueList.Count == 0)
            {
                return Task.FromResult(hasCountArg ? RedisConstants.NullArray : RedisConstants.NullBulkString);
            }

            int actualPopCount = Math.Min(requestedPopCount, valueList.Count);
            var poppedItems = valueList.GetRange(0, actualPopCount);
            valueList.RemoveRange(0, actualPopCount);

            return Task.FromResult(!hasCountArg 
                ? RESPFormatHelper.FormatBulkString(poppedItems[0]) 
                : RESPFormatHelper.FormatArray(poppedItems));
        }
    }
}
