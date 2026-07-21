using MiniRedis.Data;
using MiniRedis.Models.GlobalCache;
using System.Net.Sockets;

namespace MiniRedis.Commands
{
    public class XRangeCommand : ICommand
    {
        public int Arity => -4;

        public bool IsWriteCommand => false;

        public Task<string> ExecuteAsync(List<string> args, RedisDatabase cache, Socket client)
        {
            var cacheKey = new RedisEntry { Key = args[1] };
        }
    }
}
