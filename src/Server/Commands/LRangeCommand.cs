using System.Net.Sockets;
using Common.Helpers;
using MiniRedis.Commands.Requests;
using MiniRedis.Data;

namespace MiniRedis.Commands
{
    public class LRangeCommand : ICommand
    {
        public int Arity => -4;

        public bool IsWriteCommand => false;

        public Task<string> ExecuteAsync(List<string> args, RedisDatabase database, Socket client)
        {
            var request = LRangeRequest.Create(args);

            return Task.FromResult(
                RESPFormatHelper.FormatArray(database.LRange(request.Key, request.FromIndex, request.ToIndex)));
        }
    }
}
