using System.ComponentModel;

namespace MiniRedis.Enums
{
    public enum RedisDataType
    {
        [Description("string")]
        String,
        [Description("list")]
        List,
        [Description("set")]
        Set,
        [Description("zset")]
        ZSet,
        [Description("hash")]
        Hash,
        [Description("stream")]
        Stream,
        [Description("vectorset")]
        VectorSet
    }
}
