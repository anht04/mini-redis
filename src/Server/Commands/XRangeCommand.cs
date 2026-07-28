using Common.Helpers;
using MiniRedis.Commands.Requests;
using MiniRedis.Data;
using MiniRedis.Models;

namespace MiniRedis.Commands
{
    public class XRangeCommand : ICommand
    {
        public int Arity => -4;

        public bool IsWriteCommand => false;

        public ValueTask<CommandOutcome> TryExecuteAsync(CommandRequest request, RedisDatabase database)
        {
            var requestArgs = XRangeRequest.Create(request.Args);
            var result = database.XRange(requestArgs.Key, requestArgs.StartId, requestArgs.EndId, requestArgs.Purpose);

            return new ValueTask<CommandOutcome>(new CommandOutcome.Completed(result is null
                ? RESPFormatHelper.FormatArray((string?) null)
                : RESPFormatHelper.FormatArray(result)));
        }
    }
}