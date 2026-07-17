using System.Net.Sockets;
using Common.Constants;
using Common.Helpers;
using MiniRedis.Models;

namespace MiniRedis.Commands;

public class GetCommand : ICommand
{
    public int Arity => -2;

    public bool IsWriteCommand => false;

    public Task<string> ExecuteAsync(List<string> args, Dictionary<RedisEntry, RedisValue> cache, Socket client)
    {
        var targetKey = cache.Keys.FirstOrDefault(k => k.Key == args[1]);
        if (targetKey == null)
        {
            return Task.FromResult(RedisConstants.NullBulkString);
        }

        if (targetKey.IsExpired)
        {
            cache.Remove(targetKey);
            return Task.FromResult(RedisConstants.NullBulkString);
        }

        return Task.FromResult(RESPFormatHelper.FormatBulkString(cache[targetKey].AsString()));
    }
}