using System.Net.Sockets;
using Common.Constants;
using Common.Helpers;
using MiniRedis.Commands.AsyncManagers;
using MiniRedis.Models;
using MiniRedis.Models.GlobalCache;

namespace MiniRedis.Commands
{
    public class BLPopCommand : ICommand
    {
        public int Arity => -2;

        public bool IsWriteCommand => true;

        public async Task<string> ExecuteAsync(List<string> args, Dictionary<RedisEntry, RedisValue> cache, Socket client)
        {
            var entryDateTimeUtc = DateTimeOffset.UtcNow;
            var cacheKey = new RedisEntry { Key = args[1] };
            var hasValidTimeoutArg = float.TryParse(args[2], out var timeoutInSecondsArg);
            var currentClient = new SubscribedClient
            {
                Socket = client,
                SubscribedAt = entryDateTimeUtc,
                SubscribedTo = new TaskCompletionSource<string>(),
                TimeoutInSeconds = hasValidTimeoutArg ? timeoutInSecondsArg : null
            };

            cache.TryGetValue(cacheKey, out var redisValue);

            List<string> valueList;
            try
            {
                valueList = redisValue?.AsList() ?? [];
            }
            catch (Exception)
            {
                return await Task.FromResult(RedisErrorMessages.WrongTypeOperation);
            }

            var poppedItem = PopFromList(valueList);
            if (poppedItem != null)
            {
                return await Task.FromResult(RESPFormatHelper.FormatArray([cacheKey.Key, poppedItem]));
            }

            BlockingManager.Subscribe(cacheKey.Key, currentClient);

            var delayMilliseconds = currentClient.TimeoutInSeconds is > 0
                ? (int)(currentClient.TimeoutInSeconds.Value * 1000)
                : Timeout.Infinite;
            var timeoutDelayTask = Task.Delay(delayMilliseconds);
            var completedTask = await Task.WhenAny(currentClient.SubscribedTo.Task, timeoutDelayTask);

            if (completedTask == currentClient.SubscribedTo.Task)            {
                var item = await currentClient.SubscribedTo.Task;                
                return await Task.FromResult(RESPFormatHelper.FormatArray([cacheKey.Key, item]));
            }

            return await Task.FromResult(RedisConstants.NullArray);
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