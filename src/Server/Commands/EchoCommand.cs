using System.Net.Sockets;
using MiniRedis.Data;
using MiniRedis.Models.GlobalCache;

namespace MiniRedis.Commands;

public class EchoCommand : ICommand
{
    public int Arity => -1;

    public bool IsWriteCommand => false;

    public Task<string> ExecuteAsync(List<string> args, RedisDatabase database, Socket client)
    {
        var redisEntry = new RedisEntry { Key = args[1] };
        return Task.FromResult(RedisDatabase.Echo(redisEntry));
    }
}