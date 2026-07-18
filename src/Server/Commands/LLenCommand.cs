using System.Net.Sockets;
using Common.Helpers;
using MiniRedis.Models;

namespace MiniRedis.Commands
{
    public class LLenCommand : ICommand
    {
        public int Arity => -2;

        public bool IsWriteCommand => false;

        public Task<string> ExecuteAsync(List<string> args, Dictionary<RedisEntry, RedisValue> cache, Socket client)
        {
            var cacheKey = new RedisEntry { Key = args[1] };

            if (!cache.TryGetValue(cacheKey, out var redisValue))
            {
                return Task.FromResult(RESPFormatHelper.FormatInteger("0")); 
            }

            try
            {
                var itemCount = redisValue.AsList().Count;
                return Task.FromResult(RESPFormatHelper.FormatInteger(itemCount.ToString()));
            }
            catch (InvalidOperationException ex)
            {
                return Task.FromResult(RESPFormatHelper.FormatSimpleErrorString(ex.Message));
            }
        }
    }
}
