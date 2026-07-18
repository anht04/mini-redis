using MiniRedis.Enums;

namespace MiniRedis.Models
{
    public class RedisValue
    {
        public RedisDataType DataType { get; }
        private readonly object _value;

        public RedisValue(string value)
        {
            DataType = RedisDataType.String;
            _value = value;
        }

        public RedisValue(List<string> value)
        {
            DataType = RedisDataType.List;
            _value = value;
        }        
        
        public RedisValue(RedisStream value)
        {
            DataType = RedisDataType.Stream;
            _value = value;
        }

        private bool IsString => DataType == RedisDataType.String;
        private bool IsList => DataType == RedisDataType.List;
        private bool IsStream => DataType == RedisDataType.Stream;

        public string AsString()
        {
            if (!IsString)
            {
                throw new InvalidOperationException("Key holds the wrong kind of value");
            }
            return (string)_value;
        }

        public List<string> AsList()
        {
            if (!IsList)
            {
                throw new InvalidOperationException("Key holds the wrong kind of value");
            }
            return (List<string>)_value;
        }

        public RedisStream AsStream()
        {
            if (!IsStream)
            {
                throw new InvalidOperationException("Key holds the wrong kind of value");
            }
            return (RedisStream)_value;
        }
    }
}