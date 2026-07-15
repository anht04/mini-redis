using System.Text;

namespace Common.Helpers
{
    public static class RESPFormatHelper
    {
        public static string FormatSimpleString(string value)
        {
            return $"{RedisConstants.RESP_SimpleStringPrefix}{value}{RedisConstants.CRLF}";
        }

        public static string FormatBulkString(string value)
        {
            int length = value?.Length ?? 0;
            return $"{RedisConstants.RESP_BulkStringPrefix}{length}{RedisConstants.CRLF}{value}{RedisConstants.CRLF}";
        }

        public static string FormatErrorString(string value)
        {
            return $"{RedisConstants.RESP_ErrorResponsePrefix}{value}{RedisConstants.CRLF}";
        }

        public static string FormatRequest(string rawRequest)
        {
            var result = new StringBuilder();
            var parts = rawRequest.Split(' ');
            var prefix = GetRESPPrefix(parts.Count());

            foreach (var part in parts)
            {
                result.Append(FormatString(part));
            }

            return result.Insert(0, prefix).ToString();
        }

        private static string FormatString(string value)
        {
            return $"{RedisConstants.RESP_BulkStringPrefix}{value.Length}{RedisConstants.CRLF}{value}{RedisConstants.CRLF}";
        }

        private static string GetRESPPrefix(int length)
        {
            return $"{RedisConstants.RESP_NumberElementsInArrayPrefix}{length}{RedisConstants.CRLF}";
        }
    }
}
