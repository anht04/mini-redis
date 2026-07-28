using Common.Constants;
using Common.Helpers;
using MiniRedis.Commands.Requests;
using MiniRedis.Data;
using MiniRedis.Models;

namespace MiniRedis.Commands
{
    internal class LPopCommand : ICommand
    {
        public int Arity => -2;

        public bool IsWriteCommand => true;

        public ValueTask<CommandOutcome> TryExecuteAsync(CommandRequest request, RedisDatabase database)
        {
            var requestArgs = LPopRequest.Create(request.Args);
            var poppedItems = database.LPop(requestArgs.Key, requestArgs.Count);

            if (poppedItems.Count == 0)
            {
                return new ValueTask<CommandOutcome>(new CommandOutcome.Completed(requestArgs.HasExplicitCount 
                    ? RedisConstants.NullArray 
                    : RedisConstants.NullBulkString));
            }

            return new ValueTask<CommandOutcome>(new CommandOutcome.Completed(requestArgs.HasExplicitCount 
                ? RESPFormatHelper.FormatArray(poppedItems) 
                : RESPFormatHelper.FormatBulkString(poppedItems[0])));
        }
    }
}
