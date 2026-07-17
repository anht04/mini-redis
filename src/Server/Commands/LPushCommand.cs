using Common.Constants;
using Common.Helpers;
using MiniRedis.Commands.AsyncManagers;
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
                var waitingClient = BlockingManager.GetLongestClient(cacheKey.Key);
                if (waitingClient == null)
                {
                    cache.Add(cacheKey, new RedisValue(insertValues));
                    return RESPFormatHelper.FormatInteger(insertValues.Count);
                }

                waitingClient.SubscribedTo.SetResult(insertValues[0]);
                if (insertValues.Count > 1)
                {
                    cache.Add(cacheKey, new RedisValue(insertValues[1..]));
                    return RESPFormatHelper.FormatInteger(insertValues.Count + 1);
                }
                return RESPFormatHelper.FormatInteger(1);
            }

            List<string>? valueList;
            try
            {
                valueList = value!.AsList();
            }
            catch (Exception)
            {
                return RESPFormatHelper.FormatErrorString(RedisErrorMessages.WrongTypeOperation);
            }

            if (insertValues.Count > 1)
            {
                valueList.InsertRange(0, insertValues[1..]);
                return RESPFormatHelper.FormatInteger(valueList.Count);
            }
            else
            {
                return RESPFormatHelper.FormatInteger(valueList.Count + 1);
            }
        }
    }
}
