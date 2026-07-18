using System.Net.Sockets;
using Common.Constants;
using Common.Helpers;
using MiniRedis.Models;

namespace MiniRedis.Commands;

public class XAddCommand : ICommand
{
    public int Arity => 5;
    public bool IsWriteCommand => true;

    public async Task<string> ExecuteAsync(List<string> args, Dictionary<RedisEntry, RedisValue> cache, Socket client)
    {
        var streamEntryKey = new RedisEntry { Key = args[1] };
        var streamDataEntryId = args[2];
        var rawStreamDataValues = args[3..];

        var parsedStreamDataValues=  BuildRedisValuesFromArgs(rawStreamDataValues);
        var redisStreamData = new RedisStreamData(streamDataEntryId, parsedStreamDataValues);

        if (!cache.TryGetValue(streamEntryKey, out var value))
        {
            value = new RedisValue(new RedisStream(streamEntryKey.Key, redisStreamData));
            cache[streamEntryKey] = value;
            return await Task.FromResult(RESPFormatHelper.FormatBulkString(streamDataEntryId));
        }

        RedisStream redisStream;
        try
        {
            redisStream = value.AsStream();
        }
        catch (InvalidOperationException)
        {
            return await Task.FromResult(RedisErrorMessages.WrongTypeOperation);
        }
        
        redisStream.AddDataRange(streamEntryKey.Key, [redisStreamData]);
        return  await Task.FromResult(RESPFormatHelper.FormatBulkString(streamDataEntryId));
    }

    private static List<RedisStreamDataValue> BuildRedisValuesFromArgs(List<string> values)
    {
        List<RedisStreamDataValue> result = [];
        for (var i = 0; i < values.Count - 1; i += 2)
        {
            result.Add(new RedisStreamDataValue(values[i], values[i + 1]));
        }
        return result;
    }
}