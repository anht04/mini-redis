using System.Net.Sockets;
using Common.Helpers;
using MiniRedis.Models;

namespace MiniRedis.Commands
{
    internal class PingCommand : ICommand
    {
        public int Arity => 0;

        public bool IsWriteCommand => false;

        public Task<string> ExecuteAsync(List<string> args, Dictionary<RedisEntry, RedisValue> cache, Socket client)
        {
            return Task.FromResult(RESPFormatHelper.FormatSimpleString("PONG"));
        }
    }
}
