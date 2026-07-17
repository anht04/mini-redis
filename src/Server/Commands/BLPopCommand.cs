using MiniRedis.Models;

namespace MiniRedis.Commands
{
    public class BLPopCommand : ICommand
    {
        public int Arity => -2;
        
        public bool IsWriteCommand => true;

        public string Execute(List<string> args, Dictionary<RedisEntry, RedisValue> cache)
        {
            var cacheKey = new RedisEntry { Key = args[0] };
            
        }
    }
}