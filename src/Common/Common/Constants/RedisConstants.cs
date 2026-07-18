namespace Common.Constants
{
    public static class RedisConstants
    {
        public static string NullBulkString => $"{RESP_BulkStringPrefix}{RESP_NullResponseCode}{CRLF}";
        public static string NullArray => $"{RESP_ArrayPrefix}{RESP_NullResponseCode}{CRLF}";
        public const string CRLF = "\r\n";
        public const string RESP_SimpleStringPrefix = "+";
        public const string RESP_BulkStringPrefix = "$";
        public const string RESP_IntegerPrefix = ":";
        public const string RESP_NullResponseCode = "-1";
        public const string RESP_ArrayPrefix = "*";
    }
}
