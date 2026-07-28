using Common.Helpers;
using MiniRedis.Commands.Requests;
using MiniRedis.Data;
using MiniRedis.Models;
using System.Net.Sockets;

namespace MiniRedis.Commands;

public class XAddCommand : ICommand
{
    public int Arity => 5;
    public bool IsWriteCommand => true;

    public ValueTask<CommandOutcome> TryExecuteAsync(CommandRequest request, RedisDatabase database)
    {
        var requestArgs = XAddRequest.Create(request.Args);
        var dataId = database.XAdd(requestArgs.Key, requestArgs.DataId, requestArgs.Values);

        return new ValueTask<CommandOutcome>(new CommandOutcome.Completed(RESPFormatHelper.FormatBulkString(dataId.ToString())));
    }
}