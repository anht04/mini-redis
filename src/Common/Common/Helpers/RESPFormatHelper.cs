using Common.Constants;
using System.Text;

namespace Common.Helpers
{
    public static class RESPFormatHelper
    {
        public static string FormatArray(string? value)
        {
            if(value is null)
            {
                return FormatArrayLength(0);
            }

            var result = new StringBuilder();
            var parts = value.Split(' ');
            var prefix = FormatArrayLength(parts.Length);

            foreach (var part in parts)
            {
                result.Append(FormatBulkString(part));
            }

            return result.Insert(0, prefix).ToString();
        }

        public static string FormatArray(List<string>? values)
        {
            if(values is null)
            {
                return FormatArrayLength(0);
            }

            var result = new StringBuilder();
            var prefix = FormatArrayLength(values.Count);

            foreach (var value in values)
            {
                result.Append(FormatBulkString(value));
            }

            return result.Insert(0, prefix).ToString();
        }

        public static string FormatSimpleString(string value)
        {
            return $"{RedisConstants.RESP_SimpleStringPrefix}{value}{RedisConstants.CRLF}";
        }

        public static string FormatBulkString(string value)
        {
            int length = value?.Length ?? 0;
            return $"{RedisConstants.RESP_BulkStringPrefix}{length}{RedisConstants.CRLF}{value}{RedisConstants.CRLF}";
        }

        public static string FormatInteger(string value)
        {
            return $"{RedisConstants.RESP_IntegerPrefix}{value}{RedisConstants.CRLF}";
        }

        public static string FormatInteger(int value)
        {
            return $"{RedisConstants.RESP_IntegerPrefix}{value}{RedisConstants.CRLF}";
        }

        public static string FormatErrorString(string value)
        {
            return $"{RedisConstants.RESP_ErrorResponsePrefix}{value}{RedisConstants.CRLF}";
        }

        private static string FormatArrayLength(int length)
        {
            return $"{RedisConstants.RESP_ArrayPrefix}{length}{RedisConstants.CRLF}";
        }
    }
}
