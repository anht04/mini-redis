using Common.Constants;
using MiniRedis.Enums;
using MiniRedis.Models.GlobalCache;

namespace MiniRedis.Commands.Requests
{
    public record XRangeRequest
    {
        public RedisEntry Key { get; }
        public string StartId { get; }
        public string EndId { get; }
        public XRangeCommandPurpose Purpose { get; }

        private XRangeRequest(RedisEntry key, string startId, string endId, XRangeCommandPurpose purpose)
        {
            Key = key;
            StartId = startId;
            EndId = endId;
            Purpose = purpose;
        }

        public static XRangeRequest Create(List<string> args)
        {
            if (args.Count < 4)
            {
                throw new InvalidOperationException(RedisErrorMessages.InvalidArgument);
            }

            var startIdOrStartArgument = args[2].Trim();
            var endIdOrEndArgument = args[3].Trim();

            var purpose = XRangeCommandPurpose.NormalQuery;
            if (startIdOrStartArgument == "-")
            {
                purpose = XRangeCommandPurpose.QueryWithStartArgument;
            }

            if (endIdOrEndArgument == "+")
            {
                purpose = XRangeCommandPurpose.QueryWithEndArgument;
            }

            return new XRangeRequest(new RedisEntry { Key = args[1] }, startIdOrStartArgument, endIdOrEndArgument, purpose);
        }
    }
}
