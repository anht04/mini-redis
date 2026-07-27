using Common.Constants;
using MiniRedis.Models.GlobalCache;
using MiniRedis.Models.RedisStream;

namespace MiniRedis.Commands.Requests
{
    public record XAddRequest
    {
        public RedisEntry Key { get; }
        public string DataId { get; }
        public List<RedisStreamDataValue> Values { get; }

        private XAddRequest(RedisEntry key, string dataId, List<RedisStreamDataValue> values)
        {
            Key = key;
            DataId = dataId;
            Values = values;
        }

        public static XAddRequest Create(List<string> args)
        {
            if (args.Count < 5 || (args.Count - 3) % 2 != 0)
            {
                throw new InvalidOperationException(RedisErrorMessages.InvalidArgument);
            }

            var fieldValueArgs = args[3..];
            var values = new List<RedisStreamDataValue>(fieldValueArgs.Count / 2);
            for (var i = 0; i < fieldValueArgs.Count; i += 2)
            {
                values.Add(RedisStreamDataValue.Create(fieldValueArgs[i], fieldValueArgs[i + 1]));
            }

            return new XAddRequest(new RedisEntry { Key = args[1] }, args[2], values);
        }
    }
}
