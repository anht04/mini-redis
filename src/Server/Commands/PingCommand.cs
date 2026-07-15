using Common.Helpers;
using MiniRedis.Commands.Abstractions;

namespace MiniRedis.Commands
{
    internal class PingCommand : ICommand
    {
        public string Execute(List<string> args, Dictionary<string, string> cache)
        {
            return RESPFormatHelper.FormatSimpleString("PONG");
        }
    }
}
