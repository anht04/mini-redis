using Common.Helpers;
using MiniRedis.Commands.Abstractions;
using MiniRedis.Models;

namespace MiniRedis.Commands
{
    internal class PingCommand : ICommand
    {
        public string Execute(List<string> args, Dictionary<CacheEntry, string> cache)
        {
            return RESPFormatHelper.FormatSimpleString("PONG");
        }
    }
}
