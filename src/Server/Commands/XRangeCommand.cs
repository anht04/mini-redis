using MiniRedis.Data;
using MiniRedis.Models.GlobalCache;
using System.Net.Sockets;
using MiniRedis.Enums;

namespace MiniRedis.Commands
{
    public class XRangeCommand : ICommand
    {
        public int Arity => -4;

        public bool IsWriteCommand => false;

        public Task<string> ExecuteAsync(List<string> args, RedisDatabase database, Socket client)
        {
            var cacheKey = new RedisEntry { Key = args[1] };
            var startIdOrStartArgument = args[2].Trim();
            var endIdOrEndArgument = args[3].Trim();

            var queryPurpose = XRangeCommandPurpose.NormalQuery;
            if (startIdOrStartArgument == "-")
            {
                queryPurpose = XRangeCommandPurpose.QueryWithStartArgument;
            }

            if (endIdOrEndArgument == "+")
            {
                queryPurpose = XRangeCommandPurpose.QueryWithEndArgument;
            }
            
            return Task.FromResult(database.XRange(cacheKey, startIdOrStartArgument, endIdOrEndArgument, queryPurpose));
        }
    }
}