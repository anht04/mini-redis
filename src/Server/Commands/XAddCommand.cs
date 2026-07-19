using Common.Constants;
using Common.Helpers;
using MiniRedis.Constants;
using MiniRedis.Enums;
using MiniRedis.Models.GlobalCache;
using MiniRedis.Models.RedisStream;
using System.Net.Sockets;

namespace MiniRedis.Commands;

public class XAddCommand : ICommand
{
    public int Arity => 5;
    public bool IsWriteCommand => true;

    public async Task<string> ExecuteAsync(List<string> args, Dictionary<RedisEntry, RedisValue> cache, Socket client)
    {
        var streamEntryKey = new RedisEntry { Key = args[1] };
        var streamDataIdArg = args[2];
        var streamDataValuesArg = args[3..];

        var parsedStreamDataValues = BuildRedisValuesFromArgs(streamDataValuesArg);

        if (!cache.TryGetValue(streamEntryKey, out var value))
        {
            RedisStreamDataId dataId;
            try
            {
                dataId = AddDataRange(streamEntryKey.Key, streamDataIdArg, cache, parsedStreamDataValues);
            }
            catch (Exception e)
            {
                return await Task.FromResult(RESPFormatHelper.FormatSimpleErrorString(e.Message));
            }
            return await Task.FromResult(RESPFormatHelper.FormatBulkString(dataId.ToString()));
        }

        RedisStreamData redisStream;
        try
        {
            redisStream = value.AsStream();
        }
        catch (InvalidOperationException)
        {
            return await Task.FromResult(RedisErrorMessages.WrongTypeOperation);
        }

        RedisStreamDataId addedDataId;
        try
        {
            addedDataId = AddDataRange(streamEntryKey.Key, streamDataIdArg, cache, parsedStreamDataValues);
        }
        catch (Exception e)
        {
            return await Task.FromResult(RESPFormatHelper.FormatSimpleErrorString(e.Message));
        }
        return await Task.FromResult(RESPFormatHelper.FormatBulkString(addedDataId.ToString()));
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

    public RedisStreamDataId AddDataRange(string streamId, string dataId, Dictionary<RedisEntry, RedisValue> cache, List<RedisStreamDataValue> streamDataValues)
    {
        var newStreamData = CreateNewStreamData(streamId, dataId, streamDataValues, cache, out var idGenerationBehaviour);
        var newDataId = newStreamData.GetCurrentLargestId()!;

        var cacheKey = new RedisEntry { Key = streamId };
        var cacheValue = new RedisValue(newStreamData);

        if (!cache.TryGetValue(cacheKey, out var streamData))
        {
            cache.Add(cacheKey, cacheValue);
            ValidateDataIdForNewStream(idGenerationBehaviour, newDataId);
            return newDataId;
        }

        var parsedStreamData = streamData.AsStream();
        ValidateDataId(idGenerationBehaviour, newDataId, parsedStreamData);
        return parsedStreamData.AddRangeToNewData(newDataId, streamDataValues, idGenerationBehaviour);
    }

    private static void ValidateDataIdForNewStream(StreamDataIdGenerationBehaviour idGenerationBehaviour,
        RedisStreamDataId newDataId)
    {
        if (idGenerationBehaviour != StreamDataIdGenerationBehaviour.Manual)
        {
            return;
        }
    }

    private static void ValidateDataId(StreamDataIdGenerationBehaviour idGenerationBehaviour,
        RedisStreamDataId newDataId,
        RedisStreamData streamData)
    {
        if (idGenerationBehaviour != StreamDataIdGenerationBehaviour.Manual)
        {
            return;
        }

        if (!RedisStreamDataId.IsGreaterThan(newDataId, streamData.GetCurrentLargestId()))
        {
            throw new InvalidOperationException(RedisErrorMessages.XAddStreamDataIdSmallerThanTopItem);
        }
    }

    private RedisStreamData CreateNewStreamData(string streamId, string dataId, List<RedisStreamDataValue> streamDataValues, Dictionary<RedisEntry, RedisValue> cache, out StreamDataIdGenerationBehaviour idGenerationBehaviour)
    {
        var baseNewDataId = GenerateNewDataId(streamId, dataId, cache, out idGenerationBehaviour);
        return RedisStreamData.Create(baseNewDataId, streamDataValues, idGenerationBehaviour);
    }

    private RedisStreamDataId GenerateNewDataId(string streamId, string dataId, Dictionary<RedisEntry, RedisValue> cache,
        out StreamDataIdGenerationBehaviour generationBehaviour)
    {
        var cacheKey = new RedisEntry { Key = streamId };
        if (!RedisStreamDataId.TryParseStreamDataId(dataId, out var timestamp, out var sequence,
                out var dataIdGenerationBehaviour))
        {
            throw new InvalidOperationException("Invalid stream data id");
        }
        generationBehaviour = dataIdGenerationBehaviour;

        var hasExistingStream = cache.TryGetValue(cacheKey, out var streamData);
        var defaultSequenceForAutogeneratedSequence = timestamp!.Value == 0
            ? StreamConstants.DefaultSequenceNumberForZeroTimestamp
            : StreamConstants.DefaultSequenceNumberForNonZeroTimestamp;

        return dataIdGenerationBehaviour switch
        {
            StreamDataIdGenerationBehaviour.FullyAuto => RedisStreamDataId.Create(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                StreamConstants.DefaultSequenceNumberForNonZeroTimestamp, dataIdGenerationBehaviour),
            StreamDataIdGenerationBehaviour.AutoGeneratedSequence => hasExistingStream
                ? streamData!.AsStream().GetNextId(timestamp.Value, dataIdGenerationBehaviour)
                : RedisStreamDataId.Create(timestamp.Value, defaultSequenceForAutogeneratedSequence, dataIdGenerationBehaviour),
            StreamDataIdGenerationBehaviour.Manual => RedisStreamDataId.Create(timestamp!.Value, sequence!.Value, dataIdGenerationBehaviour),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}