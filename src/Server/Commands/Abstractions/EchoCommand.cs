using Common.Helpers;

namespace MiniRedis.Commands.Abstractions;

public class EchoCommand : ICommand
{
    public string Execute(List<string> args, Dictionary<string, string> cache)
    {
        return RESPFormatHelper.FormatBulkString(args[2]);
    }
}