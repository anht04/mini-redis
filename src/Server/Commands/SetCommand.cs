using Common.Helpers;
using MiniRedis.Data;
using MiniRedis.Models.GlobalCache;
using System.Net.Sockets;

public class SetCommand : ICommand
{
    public int Arity => -3;

    public bool IsWriteCommand => true;

    public Task<string> ExecuteAsync(List<string> args, RedisDatabase database, Socket client)
    {
        if (args.Count < 3)
        {
            return Task.FromResult(RESPFormatHelper.FormatSimpleErrorString("ERR wrong number of arguments for 'set' command"));
        }

        var key = new RedisEntry { Key = args[1] };
        var value = args[2];

        long? expireAtMs = null;

        if (args.Count >= 5)
        {
            if (!int.TryParse(args[4], out var expireDuration))
            {
                return Task.FromResult(RESPFormatHelper.FormatSimpleErrorString("ERR value is not an integer or out of range"));
            }

            expireAtMs = args[3].ToUpper() switch
            {
                "PX" => DateTimeOffset.UtcNow.AddMilliseconds(expireDuration).ToUnixTimeMilliseconds(),
                "EX" => DateTimeOffset.UtcNow.AddSeconds(expireDuration).ToUnixTimeMilliseconds(),
                _ => null
            };
        }

        var isSuccess = database.Set(key, value, expireAtMs);

        return Task.FromResult(isSuccess
            ? RESPFormatHelper.FormatSimpleString("OK")
            : RESPFormatHelper.FormatSimpleErrorString("ERR"));
    }
}