using System.Net.Sockets;
using Common.Constants;
using Common.Helpers;
using MiniRedis.Commands.Requests;
using MiniRedis.Data;
using MiniRedis.Models;

namespace MiniRedis.Commands
{
    public class BLPopCommand : ICommand
    {
        public int Arity => -2;

        public bool IsWriteCommand => true;

        public async Task<string> ExecuteAsync(List<string> args, RedisDatabase database, Socket client)
        {
            var request = BLPopRequest.Create(args);

            var currentClient = new SubscribedClient
            {
                Socket = client,
                SubscribedAt = DateTimeOffset.UtcNow,
                SubscribedTo = new TaskCompletionSource<string>(),
                TimeoutInSeconds = request.TimeoutInSeconds
            };

            var result = await database.BLPopAsync(request.Key, currentClient);

            return result is null
                ? RedisConstants.NullArray
                : RESPFormatHelper.FormatArray([result.Value.Key, result.Value.Item]);
        }
    }
}