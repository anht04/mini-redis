namespace Common.Constants
{
    public static class RedisConstants
    {
        public static string NullBulkString => $"{RESP_BulkStringPrefix}{RESP_NullBulkStringCode}{CRLF}";
        public static string NullArray => $"{RESP_ArrayPrefix}{RESP_NullBulkStringCode}{CRLF}";
        public const string CRLF = "\r\n";
        public const string RESP_SimpleStringPrefix = "+";
        public const string RESP_BulkStringPrefix = "$";
        public const string RESP_IntegerPrefix = ":";
        public const string RESP_NullSimpleStringCode = "-";
        public const string RESP_NullBulkStringCode = "-1";
        public const string RESP_ArrayPrefix = "*";
    }
}
