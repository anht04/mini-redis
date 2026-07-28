using System.Net.Sockets;
using Common.Helpers;
using MiniRedis.Commands.Requests;
using MiniRedis.Data;
using MiniRedis.Models;

namespace MiniRedis.Commands;

public class EchoCommand : ICommand
{
    public int Arity => -1;

    public bool IsWriteCommand => false;

    public ValueTask<CommandOutcome> TryExecuteAsync(CommandRequest request, RedisDatabase database)
    {
        var requestArgs = SingleKeyRequest.Create(request.Args);

        return new ValueTask<CommandOutcome>(new CommandOutcome.Completed(RESPFormatHelper.FormatBulkString(RedisDatabase.Echo(requestArgs.Key))));
    }
}