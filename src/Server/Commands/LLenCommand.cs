using System.Net.Sockets;
using MiniRedis.Commands.Requests;
using MiniRedis.Data;

namespace MiniRedis.Commands
{
    public class LLenCommand : ICommand
    {
        public int Arity => -2;

        public bool IsWriteCommand => false;

        public Task<string> ExecuteAsync(List<string> args, RedisDatabase database, Socket client)
        {
            var request = SingleKeyRequest.Create(args);

            return Task.FromResult(database.LLen(request.Key));
        }
    }
}
