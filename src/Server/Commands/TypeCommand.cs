using MiniRedis.Data;
using MiniRedis.Models.GlobalCache;
using System.Net.Sockets;

namespace MiniRedis.Commands;

public class TypeCommand : ICommand
{
    public int Arity => -3;
    public bool IsWriteCommand => false;
    public Task<string> ExecuteAsync(List<string> args, RedisDatabase database, Socket client)
    {
        var cacheKey = new RedisEntry { Key = args[1] };

        return Task.FromResult(database.Type(cacheKey));
    }
}