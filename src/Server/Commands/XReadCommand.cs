using Common.Constants;
using Common.Helpers;
using MiniRedis.Commands.AsyncManagers;
using MiniRedis.Commands.Requests;
using MiniRedis.Data;
using MiniRedis.Models;

namespace MiniRedis.Commands;

public class XReadCommand: ICommand
{
    public int Arity => -4;

    public bool IsWriteCommand => false;

    public ValueTask<CommandOutcome> TryExecuteAsync(CommandRequest request, RedisDatabase database)
    {
        var requestArgs = XReadRequest.Create(request.Args);

        if (request.IsTimedOut)
        {
            return new ValueTask<CommandOutcome>(new CommandOutcome.Completed(RedisConstants.NullArray));
        }

        var results = database.XRead(requestArgs);
        var hasData = results.Any(r => r.Data is { Count: > 0 });

        if (hasData || !requestArgs.IsBlockingRequest || request.IsRetry)
        {
            return new ValueTask<CommandOutcome>(new CommandOutcome.Completed(RESPFormatHelper.FormatArray(results)));
        }

        var subscribedClient = new SubscribedClient
        {
            SubscribedAt = DateTimeOffset.UtcNow,
            TimeoutMilliseconds = requestArgs.TimeoutInMilliseconds > 0 ? requestArgs.TimeoutInMilliseconds : null
        };

        foreach (var query in requestArgs.Queries)
        {
            BlockingManager.Subscribe(query.StreamId.Key, subscribedClient);
        }

        _ = BlockingManager.WaitThenResubmitAsync(subscribedClient, request);

        return new ValueTask<CommandOutcome>(new CommandOutcome.Pending());
    }
}