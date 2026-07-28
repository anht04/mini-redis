using Common.Helpers;
using MiniRedis.Commands.Requests;
using MiniRedis.Data;
using MiniRedis.Models;
using System.Net.Sockets;

namespace MiniRedis.Commands
{
    public class LPushCommand : ICommand
    {
        public int Arity => -3;

        public bool IsWriteCommand => true;

        public ValueTask<CommandOutcome> TryExecuteAsync(CommandRequest request, RedisDatabase database)
        {
            var requestArgs = PushRequest.Create(request.Args);

            return new ValueTask<CommandOutcome>(new CommandOutcome.Completed(RESPFormatHelper.FormatInteger(database.LPush(requestArgs.Key, requestArgs.Values))));
        }
    }
}