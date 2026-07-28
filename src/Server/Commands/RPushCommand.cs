using Common.Helpers;
using MiniRedis.Commands.Requests;
using MiniRedis.Data;
using MiniRedis.Models;

namespace MiniRedis.Commands
{
    internal class RPushCommand : ICommand
    {
        public int Arity => -3;

        public bool IsWriteCommand => true;

        public ValueTask<CommandOutcome> TryExecuteAsync(CommandRequest request, RedisDatabase database)
        {
            var requestArgs = PushRequest.Create(request.Args);

            return new ValueTask<CommandOutcome>(new CommandOutcome.Completed(RESPFormatHelper.FormatInteger(database.RPush(requestArgs.Key, requestArgs.Values))));
        }
    }
}