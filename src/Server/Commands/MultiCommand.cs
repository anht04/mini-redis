using Common.Helpers;
using MiniRedis.Commands.Requests;
using MiniRedis.Data;
using MiniRedis.Models;

namespace MiniRedis.Commands
{
    public class MultiCommand : ICommand
    {
        public int Arity => -1;

        public bool IsWriteCommand =>  false;

        public ValueTask<CommandOutcome> TryExecuteAsync(CommandRequest request, RedisDatabase database)
        {
            return new ValueTask<CommandOutcome>(new CommandOutcome.Completed(RESPFormatHelper.FormatSimpleString("OK")));
        }
    }
}
