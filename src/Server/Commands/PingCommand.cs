using Common.Helpers;
using MiniRedis.Models;

namespace MiniRedis.Commands
{
    internal class PingCommand : ICommand
    {
        public int Arity => throw new NotImplementedException();

        public bool IsWriteCommand => false;

        public string Execute(List<string> args, Dictionary<RedisEntry, RedisValue> cache)
        {
            return RESPFormatHelper.FormatSimpleString("PONG");
        }
    }
}
