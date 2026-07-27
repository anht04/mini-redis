using MiniRedis.Commands.Requests;
using MiniRedis.Data;
using System.Net.Sockets;

namespace MiniRedis.Commands
{
    public class XRangeCommand : ICommand
    {
        public int Arity => -4;

        public bool IsWriteCommand => false;

        public Task<string> ExecuteAsync(List<string> args, RedisDatabase database, Socket client)
        {
            var request = XRangeRequest.Create(args);

            return Task.FromResult(database.XRange(request.Key, request.StartId, request.EndId, request.Purpose));
        }
    }
}