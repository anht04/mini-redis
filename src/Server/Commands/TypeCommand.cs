using System.Net.Sockets;
using Common.Constants;
using Common.Helpers;
using MiniRedis.Enums;
using MiniRedis.Extensions;
using MiniRedis.Models.GlobalCache;

namespace MiniRedis.Commands;

public class TypeCommand : ICommand
{
    public int Arity => -3;
    public bool IsWriteCommand => false;
    public Task<string> ExecuteAsync(List<string> args, Dictionary<RedisEntry, RedisValue> cache, Socket client)
    {
        var cacheKey = new RedisEntry { Key = args[1] };
        cache.TryGetValue(cacheKey, out var value);

        if (value is null)
        {
            return Task.FromResult(RESPFormatHelper.FormatSimpleString(RedisErrorMessages.EnumValueNotFound));
        }

        var typeString = value.DataType switch
        {
            RedisDataType.String => RedisDataType.String.GetDescription(),
            RedisDataType.List => RedisDataType.List.GetDescription(),
            RedisDataType.Set => RedisDataType.Set.GetDescription(),
            RedisDataType.ZSet => RedisDataType.ZSet.GetDescription(),
            RedisDataType.Hash => RedisDataType.Hash.GetDescription(),
            RedisDataType.Stream => RedisDataType.Stream.GetDescription(),
            RedisDataType.VectorSet => RedisDataType.VectorSet.GetDescription(),
            _ => RedisErrorMessages.EnumValueNotFound
        };

        return Task.FromResult(RESPFormatHelper.FormatSimpleString(typeString));
    }
}