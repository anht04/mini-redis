using Common.Helpers;
using MiniRedis.Models;

namespace MiniRedis.Commands
{
    public class LLenCommand : ICommand
    {
        public int Arity => -2;

        public bool IsWriteCommand => false;

        public string Execute(List<string> args, Dictionary<RedisEntry, RedisValue> cache)
        {
            var cacheKey = new RedisEntry { Key = args[1] };

            if (!cache.TryGetValue(cacheKey, out var redisValue))
            {
                return RESPFormatHelper.FormatInteger("0"); 
            }

            try
            {
                var itemCount = redisValue.AsList().Count;
                return RESPFormatHelper.FormatInteger(itemCount.ToString());
            }
            catch (InvalidOperationException ex)
            {
                return RESPFormatHelper.FormatErrorString(ex.Message);
            }
        }
    }
}
