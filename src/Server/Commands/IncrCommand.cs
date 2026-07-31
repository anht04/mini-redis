using Common.Helpers;
using MiniRedis.Commands.Requests;
using MiniRedis.Data;
using MiniRedis.Models;

namespace MiniRedis.Commands
{
    public class IncrCommand : ICommand
    {
        public int Arity => -1;

        public bool IsWriteCommand => true;

        public ValueTask<CommandOutcome> TryExecuteAsync(CommandRequest request, RedisDatabase database)
        {
            var requestArgs = SingleKeyRequest.Create(request.Args);
            return new ValueTask<CommandOutcome>(new CommandOutcome.Completed(RESPFormatHelper.FormatInteger(database.Incr(requestArgs.Key))));
        }
    }
}
