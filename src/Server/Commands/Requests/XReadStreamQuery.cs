using MiniRedis.Models.GlobalCache;
using MiniRedis.Models.RedisStream;

namespace MiniRedis.Commands.Requests
{
    public record XReadStreamQuery
    {
        public RedisEntry StreamId { get; set; }
        public RedisStreamDataId DataId { get; set; }

        private XReadStreamQuery(RedisEntry key, RedisStreamDataId value)
        {
            StreamId = key;
            DataId = value;
        }

        public static XReadStreamQuery Create(RedisEntry key, RedisStreamDataId value)
        {
            return new XReadStreamQuery(key, value);
        }

        public string[] ToKeyValueStringArray()
        {
            return [StreamId.Key, DataId.ToString()];
        }
    }
}
