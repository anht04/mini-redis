using Common.Helpers;
using MiniRedis.Commands.Requests;
using MiniRedis.Data;
using MiniRedis.Models;
using System.Net.Sockets;

namespace MiniRedis.Commands
{
    public class LRangeCommand : ICommand
    {
        public int Arity => -4;

        public bool IsWriteCommand => false;

        public ValueTask<CommandOutcome> TryExecuteAsync(CommandRequest request, RedisDatabase database)
        {
            var requestArgs = LRangeRequest.Create(request.Args);

            return new ValueTask<CommandOutcome>(new CommandOutcome.Completed(
                RESPFormatHelper.FormatArray(database.LRange(requestArgs.Key, requestArgs.FromIndex, requestArgs.ToIndex))));
        }
    }
}
