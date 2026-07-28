using Common.Helpers;
using MiniRedis.Commands.Requests;
using MiniRedis.Data;
using MiniRedis.Models;
using System.Net.Sockets;

namespace MiniRedis.Commands;

public class TypeCommand : ICommand
{
    public int Arity => -3;
    public bool IsWriteCommand => false;
    public ValueTask<CommandOutcome> TryExecuteAsync(CommandRequest request, RedisDatabase database)
    {
        var requestArgs = SingleKeyRequest.Create(request.Args);

        return new ValueTask<CommandOutcome>(new CommandOutcome.Completed(RESPFormatHelper.FormatSimpleString(database.Type(requestArgs.Key))));
    }
}