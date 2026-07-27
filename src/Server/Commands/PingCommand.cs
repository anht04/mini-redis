using System.Net.Sockets;
using Common.Constants;
using Common.Helpers;
using MiniRedis.Data;

namespace MiniRedis.Commands
{
    internal class PingCommand : ICommand
    {
        public int Arity => 0;

        public bool IsWriteCommand => false;

        public Task<string> ExecuteAsync(List<string> args, RedisDatabase database, Socket client)
        {
            if (args.Count != 1)
            {
                throw new InvalidOperationException(RedisErrorMessages.InvalidArgument);
            }

            return Task.FromResult(RESPFormatHelper.FormatSimpleString("PONG"));
        }
    }
}
