using Common.Constants;
using Common.Helpers;
using MiniRedis.Models;

namespace MiniRedis.Commands
{
    public class LPushCommand : ICommand
    {
        public int Arity => -3;

        public bool IsWriteCommand => true;

        public string Execute(List<string> args, Dictionary<RedisEntry, RedisValue> cache)
        {
            var cacheKey = new RedisEntry { Key = args[1] };

            var insertValues = args.GetRange(2, args.Count - 2);
            insertValues.Reverse();

            if (!cache.TryGetValue(cacheKey, out var value))
            {
                cache.Add(cacheKey, new RedisValue(insertValues));
                return RESPFormatHelper.FormatInteger(insertValues.Count);
            }

            List<string>? valueList;
            try
            {
                valueList = value.AsList();
            }
            catch (Exception)
            {
                return RESPFormatHelper.FormatErrorString(RedisErrorMessages.WrongTypeOperation);
            }

            valueList.InsertRange(0, insertValues);

            return RESPFormatHelper.FormatInteger(valueList.Count);
        }
    }
}
