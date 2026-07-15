using Common;
using Common.Helpers;
using MiniRedis.Commands.Abstractions;

namespace MiniRedis.Commands;

public class GetCommand : ICommand
{
    public string Execute(List<string> args, Dictionary<string, string> cache)
    {
        return cache.TryGetValue(args[4], out var result)
            ? RESPFormatHelper.FormatBulkString(result)
            : RedisConstants.NullBulkString;
    }
}