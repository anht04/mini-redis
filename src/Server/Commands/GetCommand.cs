using Common;
using Common.Helpers;
using MiniRedis.Models;

namespace MiniRedis.Commands;

public class GetCommand : ICommand
{
    public int Arity => -2;

    public bool IsWriteCommand => false;

    public string Execute(List<string> args, Dictionary<RedisEntry, RedisValue> cache)
    {
        var targetKey = cache.Keys.FirstOrDefault(k => k.Key == args[1]);
        if (targetKey == null)
        {
            return RedisConstants.NullBulkString;
        }

        if (targetKey.IsExpired)
        {
            cache.Remove(targetKey);
            return RedisConstants.NullBulkString;
        }

        return RESPFormatHelper.FormatBulkString(cache[targetKey].AsString());
    }
}