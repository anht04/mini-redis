using Common.Helpers;
using MiniRedis.Commands.Requests;
using MiniRedis.Data;
using MiniRedis.Models;
using System.Net.Sockets;

public class SetCommand : ICommand
{
    public int Arity => -3;

    public bool IsWriteCommand => true;

    public ValueTask<CommandOutcome> TryExecuteAsync(CommandRequest request, RedisDatabase database)
    {
        var requestArgs = SetRequest.Create(request.Args);

        var isSuccess = database.Set(requestArgs.CacheKey, requestArgs.Value);

        return new ValueTask<CommandOutcome>(new CommandOutcome.Completed(isSuccess
            ? RESPFormatHelper.FormatSimpleString("OK")
            : RESPFormatHelper.FormatSimpleErrorString("ERR")));
    }
}