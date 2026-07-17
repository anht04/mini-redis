using System.Net.Sockets;
using MiniRedis.Models;

namespace MiniRedis.Commands.Abstractions
{
    public interface ICommand
    {
        int Arity { get; }
        bool IsWriteCommand { get; }
        Task<string> ExecuteAsync(List<string> args, Dictionary<RedisEntry, RedisValue> cache, Socket client);
    }
}
