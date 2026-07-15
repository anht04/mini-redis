using Common;
using Common.Helpers;
using MiniRedis.Commands.Abstractions;

namespace MiniRedis.Commands;

public class SetCommand : ICommand
{
    public string Execute(List<string> args, Dictionary<string, string> cache)
    {
        return cache.TryAdd(args[2], args[4])
            ? RESPFormatHelper.FormatSimpleString("OK")
            : RESPFormatHelper.FormatSimpleString("ERR");
    }
}