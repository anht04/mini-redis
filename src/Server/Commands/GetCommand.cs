using Common.Constants;
using Common.Helpers;
using MiniRedis.Commands.Requests;
using MiniRedis.Data;
using MiniRedis.Models;

namespace MiniRedis.Commands;

public class GetCommand : ICommand
{
    public int Arity => -2;

    public bool IsWriteCommand => false;

    public ValueTask<CommandOutcome> TryExecuteAsync(CommandRequest request, RedisDatabase database)
    {
        var requestArgs = SingleKeyRequest.Create(request.Args);
        var value = database.Get(requestArgs.Key);

        return new ValueTask<CommandOutcome>(new CommandOutcome.Completed(value is null
            ? RedisConstants.NullBulkString
            : RESPFormatHelper.FormatBulkString(value)));
    }
}