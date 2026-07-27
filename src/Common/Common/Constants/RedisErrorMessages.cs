namespace Common.Constants
{
    public static class RedisErrorMessages
    {
        public const string WrongTypeOperation = "WRONGTYPE Operation against a key holding the wrong kind of value";
        public const string InvalidArgument = "ERR The provided arguments does not match required length or are in invalid format";
        public const string XAddStreamDataIdSmallerThanTopItem = "ERR The ID specified in XADD is equal or smaller than the target stream top item";
        public const string XAddStreamDataIdNotGreaterThan0 = "ERR The ID specified in XADD must be greater than 0-0";
        public const string XAddStreamDataIdInvalidFormat = "ERR The ID is in invalid format";
        public const string EnumValueNotFound = "none";
        public static class XRead
        {
            public const string UnbalancedXREADArgs = "ERR Unbalanced XREAD list of streams: for each stream key an ID or '$' must be specified.";
        }
    }
}
