using Common.Constants;
using Common.Helpers;
using MiniRedis.Commands.Requests;
using MiniRedis.Data;
using MiniRedis.Models;
using System.Net.Sockets;

namespace MiniRedis.Commands
{
    internal class PingCommand : ICommand
    {
        public int Arity => 0;

        public bool IsWriteCommand => false;

        public ValueTask<CommandOutcome> TryExecuteAsync(CommandRequest request, RedisDatabase database)
        {
            if (request.Args.Count != 1)
            {
                throw new InvalidOperationException(RedisErrorMessages.InvalidArgument);
            }

            return new ValueTask<CommandOutcome>(new CommandOutcome.Completed(RESPFormatHelper.FormatSimpleString("PONG")));
        }
    }
}
