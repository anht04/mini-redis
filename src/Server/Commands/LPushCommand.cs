using System.Net.Sockets;
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

        public Task<string> ExecuteAsync(List<string> args, Dictionary<RedisEntry, RedisValue> cache, Socket client)
        {
            var cacheKey = new RedisEntry { Key = args[1] };

            var insertValues = args.GetRange(2, args.Count - 2);
            insertValues.Reverse();

            var waitingClient = BlockingManager.GetLongestClient(cacheKey.Key);
            if (waitingClient == null)
            {
                return PushToCacheAndFormat(insertValues, cacheKey, cache);
            }

            waitingClient.SubscribedTo.SetResult(insertValues[0]);
            if (insertValues.Count == 1)
            {
                return Task.FromResult(RESPFormatHelper.FormatInteger(1));
            }

            var remainingValues = insertValues.Skip(1).ToList();
            return PushToCacheAndFormat(remainingValues, cacheKey, cache, 1);
        }

        private static Task<string> PushToCacheAndFormat(List<string> values, RedisEntry cacheKey,
            Dictionary<RedisEntry, RedisValue> cache, int valuesSentToClientCount = 0)
        {
            if (!cache.TryGetValue(cacheKey, out var value))
            {
                cache.Add(cacheKey, new RedisValue(values));
                return Task.FromResult(RESPFormatHelper.FormatInteger(values.Count + valuesSentToClientCount));
            }

            List<string> valueList;
            try
            {
                valueList = value.AsList();
            }
            catch (InvalidOperationException)
            {
                return Task.FromResult(RESPFormatHelper.FormatErrorString(RedisErrorMessages.WrongTypeOperation));
            }

            valueList.InsertRange(0, values);
            return Task.FromResult(RESPFormatHelper.FormatInteger(valueList.Count + valuesSentToClientCount));
        }
    }
}