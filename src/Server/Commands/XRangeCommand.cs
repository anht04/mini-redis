using MiniRedis.Data;
using MiniRedis.Models.GlobalCache;
using System.Net.Sockets;

namespace MiniRedis.Commands
{
    public class XRangeCommand : ICommand
    {
        public int Arity => -4;

        public bool IsWriteCommand => false;

        public Task<string> ExecuteAsync(List<string> args, RedisDatabase database, Socket client)
        {
            var cacheKey = new RedisEntry { Key = args[1] };
            var startId = args[2];
            var endId = args[3];

            return Task.FromResult(database.XRange(cacheKey, startId, endId));
        }
    }
}
