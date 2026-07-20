using System.Net.Sockets;
using MiniRedis.Data;
using MiniRedis.Models.GlobalCache;

namespace MiniRedis.Commands
{
    internal class RPushCommand : ICommand
    {
        public int Arity => -3;

        public bool IsWriteCommand => true;

        public Task<string> ExecuteAsync(List<string> args, RedisDatabase database, Socket client)
        {
            var insertValues = args[2..];
            var cacheKey = new RedisEntry { Key = args[1] };

            return Task.FromResult(database.RPush(cacheKey, insertValues));
        }
    }
}