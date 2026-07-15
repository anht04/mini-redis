using Common.Helpers;
using MiniRedis.Commands.Abstractions;
using MiniRedis.Models;

namespace MiniRedis.Commands;

public class EchoCommand : ICommand
{
    public string Execute(List<string> args, Dictionary<CacheEntry, string> cache)
    {
        return RESPFormatHelper.FormatBulkString(args[1]);
    }
}