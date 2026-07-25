using Common.Constants;
using MiniRedis.Constants;
using MiniRedis.Data;
using MiniRedis.Enums;
using MiniRedis.Models.GlobalCache;
using System.Net.Sockets;
using MiniRedis.Models.RedisStream;

namespace MiniRedis.Commands;

public class XAddCommand : ICommand
{
    public int Arity => 5;
    public bool IsWriteCommand => true;

    public async Task<string> ExecuteAsync(List<string> args, RedisDatabase database, Socket client)
    {
        var streamEntryKey = new RedisEntry { Key = args[1] };
        var streamDataId = args[2];
        var streamDataValuesArg = args[3..];

        var parsedStreamDataValues = BuildRedisValuesFromArgs(streamDataValuesArg);

        return await Task.FromResult(database.XAdd(streamEntryKey, streamDataId, parsedStreamDataValues));
    }

    private static List<RedisStreamDataValue> BuildRedisValuesFromArgs(List<string> values)
    {
        List<RedisStreamDataValue> result = [];
        for (var i = 0; i < values.Count - 1; i += 2)
        {
            result.Add(RedisStreamDataValue.Create(values[i], values[i + 1]));
        }

        return result;
    }
}