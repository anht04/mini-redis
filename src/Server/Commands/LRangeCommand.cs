using System.Net.Sockets;
using MiniRedis.Data;
using MiniRedis.Models.GlobalCache;

namespace MiniRedis.Commands
{
    public class LRangeCommand : ICommand
    {
        public int Arity => -4;

        public bool IsWriteCommand => false;

        public Task<string> ExecuteAsync(List<string> args, RedisDatabase database, Socket client)
        {
            var cacheKey = new RedisEntry { Key = args[1] };
            var fromIndex = int.Parse(args[2]);
            var toIndex = int.Parse(args[3]);

            return Task.FromResult(database.LRange(cacheKey, fromIndex, toIndex));
        }
    }
}
