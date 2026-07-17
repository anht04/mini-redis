namespace Common.Constants
{
    public static class RedisConstants
    {
        public static string NullBulkString => $"{RESP_BulkStringPrefix}{RESP_ErrorResponsePrefix}1{CRLF}";
        public static string NullArray => $"{RESP_ArrayPrefix}0{CRLF}";
        public const string CRLF = "\r\n";
        public const string RESP_SimpleStringPrefix = "+";
        public const string RESP_BulkStringPrefix = "$";
        public const string RESP_IntegerPrefix = ":";
        public const string RESP_ErrorResponsePrefix = "-";
        public const string RESP_ArrayPrefix = "*";
    }
}
