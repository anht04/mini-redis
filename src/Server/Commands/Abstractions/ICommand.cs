using System.Net.Sockets;
using MiniRedis.Data;

namespace MiniRedis.Commands.Abstractions
{
    public interface ICommand
    {
        int Arity { get; }
        bool IsWriteCommand { get; }
        Task<string> ExecuteAsync(List<string> args, RedisDatabase cache, Socket client);
    }
}
