using System.Net.Sockets;
using MiniRedis.Commands.Requests;
using MiniRedis.Data;

namespace MiniRedis.Commands
{
    internal class RPushCommand : ICommand
    {
        public int Arity => -3;

        public bool IsWriteCommand => true;

        public Task<string> ExecuteAsync(List<string> args, RedisDatabase database, Socket client)
        {
            var request = PushRequest.Create(args);

            return Task.FromResult(database.RPush(request.Key, request.Values));
        }
    }
}