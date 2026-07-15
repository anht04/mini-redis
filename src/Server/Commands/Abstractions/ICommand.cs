using MiniRedis.Models;

namespace MiniRedis.Commands.Abstractions
{
    public interface ICommand
    {
        string Execute(List<string> args, Dictionary<CacheEntry, string> cache);
    }
}
