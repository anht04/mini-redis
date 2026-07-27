using Common.Helpers;
using MiniRedis.Commands.Requests;
using MiniRedis.Data;
using System.Net.Sockets;

public class SetCommand : ICommand
{
    public int Arity => -3;

    public bool IsWriteCommand => true;

    public Task<string> ExecuteAsync(List<string> args, RedisDatabase database, Socket client)
    {
        var request = SetRequest.Create(args);

        var isSuccess = database.Set(request.CacheKey, request.Value);

        return Task.FromResult(isSuccess
            ? RESPFormatHelper.FormatSimpleString("OK")
            : RESPFormatHelper.FormatSimpleErrorString("ERR"));
    }
}