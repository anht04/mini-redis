using System.Net.Sockets;
using MiniRedis.Data;
using MiniRedis.Models.GlobalCache;

namespace MiniRedis.Commands
{
    public class LLenCommand : ICommand
    {
        public int Arity => -2;

        public bool IsWriteCommand => false;

        public Task<string> ExecuteAsync(List<string> args, RedisDatabase database, Socket client)
        {
            var cacheKey = new RedisEntry { Key = args[1] };

            return Task.FromResult(database.LLen(cacheKey));
        }
    }
}
