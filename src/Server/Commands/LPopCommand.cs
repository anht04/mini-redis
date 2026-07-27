using System.Net.Sockets;
using MiniRedis.Commands.Requests;
using MiniRedis.Data;

namespace MiniRedis.Commands
{
    internal class LPopCommand : ICommand
    {
        public int Arity => -2;

        public bool IsWriteCommand => true;

        public Task<string> ExecuteAsync(List<string> args, RedisDatabase database, Socket client)
        {
            var request = LPopRequest.Create(args);

            return Task.FromResult(database.LPop(request.Key, request.Count));
        }
    }
}
