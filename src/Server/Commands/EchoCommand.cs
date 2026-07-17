using System.Net.Sockets;
using Common.Helpers;
using MiniRedis.Models;

namespace MiniRedis.Commands;

public class EchoCommand : ICommand
{
    public int Arity => -1;

    public bool IsWriteCommand => false;

    public Task<string> ExecuteAsync(List<string> args, Dictionary<RedisEntry, RedisValue> cache, Socket client)
    {
        return Task.FromResult(RESPFormatHelper.FormatBulkString(args[1]));
    }
}