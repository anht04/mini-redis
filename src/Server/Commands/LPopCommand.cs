using System.Net.Sockets;
using MiniRedis.Data;
using MiniRedis.Models.GlobalCache;

namespace MiniRedis.Commands
{
    internal class LPopCommand : ICommand
    {
        public int Arity => -2;

        public bool IsWriteCommand => true;

        public Task<string> ExecuteAsync(List<string> args, RedisDatabase database, Socket client)
        {
            var cacheKey = new RedisEntry { Key = args[1] };
            bool hasCountArg = args.Count > 1;
            int requestedPopCount = hasCountArg ? int.Parse(args[2]) : 1;

            return Task.FromResult(database.LPop(cacheKey, requestedPopCount));
        }
    }
}
