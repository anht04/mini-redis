using System.Net.Sockets;
using Common.Constants;
using Common.Helpers;
using MiniRedis.Commands.Requests;
using MiniRedis.Data;

namespace MiniRedis.Commands;

public class GetCommand : ICommand
{
    public int Arity => -2;

    public bool IsWriteCommand => false;

    public Task<string> ExecuteAsync(List<string> args, RedisDatabase database, Socket client)
    {
        var request = SingleKeyRequest.Create(args);
        var value = database.Get(request.Key);

        return Task.FromResult(value is null
            ? RedisConstants.NullBulkString
            : RESPFormatHelper.FormatBulkString(value));
    }
}