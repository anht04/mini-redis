using System.Net.Sockets;
using MiniRedis.Data;
using MiniRedis.Models;
using MiniRedis.Models.GlobalCache;

namespace MiniRedis.Commands
{
    public class BLPopCommand : ICommand
    {
        public int Arity => -2;

        public bool IsWriteCommand => true;

        public Task<string> ExecuteAsync(List<string> args, RedisDatabase database, Socket client)
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

            return database.BLPopAsync(cacheKey, currentClient);
        }
    }
}