using System.Net.Sockets;
using Common.Helpers;
using MiniRedis.Models.GlobalCache;

namespace MiniRedis.Commands;

public class SetCommand : ICommand
{
    public int Arity => throw new NotImplementedException();

    public bool IsWriteCommand => true;

    public Task<string> ExecuteAsync(List<string> args, Dictionary<RedisEntry, RedisValue> cache, Socket client)
    {
        if (args.Count < 3)
        {
            return Task.FromResult(RESPFormatHelper.FormatSimpleErrorString("ERR wrong number of arguments for 'set' command"));
        }
        
        var key = args[1];
        var rawValue = args[2];

        DateTimeOffset? expireAt = null;
        
        if (args.Count > 4)
        {
            if(!int.TryParse(args[4], out var expireDuration))
            {
                return Task.FromResult(RESPFormatHelper.FormatSimpleString("ERR"));
            }

            expireAt = args[3].ToUpper() switch
            {
                "PX" => DateTimeOffset.UtcNow.AddMilliseconds(expireDuration),
                "EX" => DateTimeOffset.UtcNow.AddSeconds(expireDuration),
                _ => null
            };
        }

        var cacheEntry = new RedisEntry
        {
            Key = key,
            ExpireAtMs = expireAt?.ToUnixTimeMilliseconds()
        };
        
        return Task.FromResult(cache.TryAdd(cacheEntry, new RedisValue(rawValue))
            ? RESPFormatHelper.FormatSimpleString("OK")
            : RESPFormatHelper.FormatSimpleString("ERR"));
    }
}