using Common.Helpers;
using MiniRedis.Commands.Requests;
using MiniRedis.Data;
using MiniRedis.Models;

namespace MiniRedis.Commands
{
    public class LLenCommand : ICommand
    {
        public int Arity => -2;

        public bool IsWriteCommand => false;

        public ValueTask<CommandOutcome> TryExecuteAsync(CommandRequest request, RedisDatabase database)
        {
            var requestArgs = SingleKeyRequest.Create(request.Args);

            return new ValueTask<CommandOutcome>(new CommandOutcome.Completed(RESPFormatHelper.FormatInteger(database.LLen(requestArgs.Key))));
        }
    }
}
