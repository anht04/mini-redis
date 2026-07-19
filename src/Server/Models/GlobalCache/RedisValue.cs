using Common.Constants;
using MiniRedis.Enums;
using MiniRedis.Models.RedisStream;

namespace MiniRedis.Models.GlobalCache
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
        
        public RedisValue(RedisStreamData value)
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
                throw new InvalidOperationException(RedisErrorMessages.WrongTypeOperation);
            }
            return (string)_value;
        }

        public List<string> AsList()
        {
            if (!IsList)
            {
                throw new InvalidOperationException(RedisErrorMessages.WrongTypeOperation);
            }
            return (List<string>)_value;
        }

        public RedisStreamData AsStream()
        {
            if (!IsStream)
            {
                throw new InvalidOperationException(RedisErrorMessages.WrongTypeOperation);
            }
            return (RedisStreamData)_value;
        }
    }
}