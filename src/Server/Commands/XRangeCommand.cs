using Common.Helpers;
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
            var result = database.XRange(request.Key, request.StartId, request.EndId, request.Purpose);

            return Task.FromResult(result is null
                ? RESPFormatHelper.FormatArray((string?)null)
                : RESPFormatHelper.FormatArray(result));
        }
    }
}