using System.Net.Sockets;
using MiniRedis.Commands.Requests;
using MiniRedis.Data;
using MiniRedis.Models;

namespace MiniRedis.Commands
{
    public class BLPopCommand : ICommand
    {
        public int Arity => -2;

        public bool IsWriteCommand => true;

        public Task<string> ExecuteAsync(List<string> args, RedisDatabase database, Socket client)
        {
            var request = BLPopRequest.Create(args);

            var currentClient = new SubscribedClient
            {
                Socket = client,
                SubscribedAt = DateTimeOffset.UtcNow,
                SubscribedTo = new TaskCompletionSource<string>(),
                TimeoutInSeconds = request.TimeoutInSeconds
            };

            return database.BLPopAsync(request.Key, currentClient);
        }
    }
}