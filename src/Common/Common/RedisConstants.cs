namespace Common
{
    public static class RedisConstants
    {
        public static string NullBulkString => $"{RESP_BulkStringPrefix}{RESP_ErrorResponsePrefix}1{CRLF}";
        public const string CRLF = "\r\n";
        public const string RESP_SimpleStringPrefix = "+";
        public const string RESP_BulkStringPrefix = "$";
        public const string RESP_ErrorResponsePrefix = "-";
        public const string RESP_NumberElementsInArrayPrefix = "*";

    }
}
