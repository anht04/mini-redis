using Common.Helpers;
using MiniRedis.Models;

namespace MiniRedis.Commands
{
    internal class RPushCommand : ICommand
    {
        public int Arity => -3;

        public bool IsWriteCommand => true;

        public string Execute(List<string> args, Dictionary<RedisEntry, RedisValue> cache)
        {
            var collectionKey = args[1];
            var newValues = args[2..];
            var key = new RedisEntry { Key = collectionKey };

            if (cache.TryGetValue(key, out var value))
            {
                return RESPFormatHelper.FormatInteger(value.AppendToList(newValues).ToString());
            }
            else
            {
                cache.Add(key, new RedisValue([..newValues]));

                return RESPFormatHelper.FormatInteger(cache[key].GetListSize().ToString());
            }
        }
    }
}
