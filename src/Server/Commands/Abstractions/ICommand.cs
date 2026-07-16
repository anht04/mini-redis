using MiniRedis.Models;

namespace MiniRedis.Commands.Abstractions
{
    public interface ICommand
    {
        int Arity { get; }
        bool IsWriteCommand { get; }
        string Execute(List<string> args, Dictionary<RedisEntry, RedisValue> cache);
    }
}
