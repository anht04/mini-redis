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

        public bool IsString => DataType == RedisDataType.String;
        public bool IsList => DataType == RedisDataType.List;

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
    }
}