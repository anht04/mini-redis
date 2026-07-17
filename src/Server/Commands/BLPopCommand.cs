using Common.Constants;
using Common.Helpers;
using MiniRedis.Models;

namespace MiniRedis.Commands
{
    public class BLPopCommand : ICommand
    {
        public int Arity => -2;

        public bool IsWriteCommand => true;

        public string Execute(List<string> args, Dictionary<RedisEntry, RedisValue> cache)
        {
            var entryDateTimeUtc = DateTimeOffset.UtcNow;
            var cacheKey = new RedisEntry { Key = args[0] };
            var hasValidTimeoutArg = int.TryParse(args[1], out int timeoutInSecondsArg);

            if (!cache.TryGetValue(cacheKey, out var redisValue))
            {
                return RedisConstants.NullArray;
            }

            List<string> valueList;
            try
            {
                valueList = redisValue.AsList();
            }
            catch (Exception)
            {
                return RedisErrorMessages.WrongTypeOperation;
            }

            var poppedItem = PopFromList(valueList);
            if (poppedItem != null)
            {
                return RESPFormatHelper.FormatArray(poppedItem);
            }

            while (!hasValidTimeoutArg || (DateTimeOffset.UtcNow - entryDateTimeUtc).TotalSeconds >= timeoutInSecondsArg)
            {
                if (valueList.Count == 0)
                {
                    return RedisConstants.NullArray;
                }

                var item = PopFromList(valueList);
                if (item != null)
                {
                    return RESPFormatHelper.FormatArray(item);
                }
            }

            return RedisConstants.NullArray;
        }

        private static string? PopFromList(List<string> list)
        {
            if (list.Count == 0)
            {
                return null;
            }
            var poppedItem = list[0];
            list.RemoveAt(0);
            return poppedItem;
        }
    }
}