using System.Net.Sockets;
using Common.Constants;
using Common.Helpers;
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
            var poppedItems = database.LPop(request.Key, request.Count);

            if (poppedItems.Count == 0)
            {
                return Task.FromResult(request.HasExplicitCount ? RedisConstants.NullArray : RedisConstants.NullBulkString);
            }

            return Task.FromResult(request.HasExplicitCount
                ? RESPFormatHelper.FormatArray(poppedItems)
                : RESPFormatHelper.FormatBulkString(poppedItems[0]));
        }
    }
}
