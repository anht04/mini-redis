using Common.Constants;
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
                List<string>? parsedValue;
                try
                {
                    parsedValue = value.AsList();
                }
                catch (Exception)
                {
                    return RESPFormatHelper.FormatErrorString(RedisErrorMessages.WrongTypeOperation);
                }
                parsedValue.AddRange(newValues);
                return RESPFormatHelper.FormatInteger(parsedValue.Count);
            }

            cache.Add(key, new RedisValue(newValues));
            return RESPFormatHelper.FormatInteger(newValues.Count);
        }
    }
}
