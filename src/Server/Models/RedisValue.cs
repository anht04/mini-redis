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

        public string AsString()
        {
            if (DataType != RedisDataType.String)
            {
                throw new ArgumentException();
            }
            return (string)_value;
        }

        public List<string> AsList()
        {
            if (DataType != RedisDataType.List)
            {
                throw new ArgumentException();
            }
            return (List<string>)_value;
        }

        public int AppendToList(string value)
        {
            if (DataType != RedisDataType.List)
            {
                throw new ArgumentException();
            }

            var currentValue = (List<string>)_value ?? [];
            currentValue.Add(value);
            return currentValue.Count;
        }

        public int AppendToList(List<string> values)
        {
            if (DataType != RedisDataType.List)
            {
                throw new ArgumentException();
            }

            var currentValue = (List<string>)_value ?? [];
            currentValue.AddRange(values);
            return currentValue.Count;
        }

        public int GetListSize()
        {
            if (DataType != RedisDataType.List)
            {
                throw new ArgumentException();
            }

            return ((List<string>)_value).Count;
        }
    }
}
