using Common.Constants;
using Common.Helpers;
using MiniRedis.Commands.AsyncManagers;
using MiniRedis.Commands.Requests;
using MiniRedis.Data;
using MiniRedis.Models;
using MiniRedis.Models.GlobalCache;

namespace MiniRedis.Commands
{
    public class BLPopCommand : ICommand
    {
        public int Arity => -2;

        public bool IsWriteCommand => true;


        public ValueTask<CommandOutcome> TryExecuteAsync(CommandRequest request, RedisDatabase database)
        {
            // ANHT summarization:
            // 1. Try to Pop synchronously no matter this is the first time or retry. If has data => directly return.
            // 2. If this is a retry turn (which means timeout or another client Popped the result) => return Null array.
            // 3. If this is the first time and the store is currently empty => Subscribe and wait
            // 4. Activate the wait Task and return Pending status to allow Event Loop can be able to process other clients' requests.

            var blPopRequest = BLPopRequest.Create(request.Args);

            if(request.IsTimedOut)
            {
                return new ValueTask<CommandOutcome>(new CommandOutcome.Completed(RedisConstants.NullArray));
            }

            var poppedItem = database.BLPop(new RedisEntry { Key = blPopRequest.Key.Key });
            if (poppedItem is not null)
            {
                return new ValueTask<CommandOutcome>(new CommandOutcome.Completed(
                    RESPFormatHelper.FormatArray([blPopRequest.Key.Key, poppedItem])));
            }

            if(request.IsRetry)
            {
                return new ValueTask<CommandOutcome>(new CommandOutcome.Completed(RedisConstants.NullArray));
            }

            var subscribedClient = new SubscribedClient
            {
                SubscribedAt = DateTimeOffset.UtcNow,
                TimeoutMilliseconds = blPopRequest.TimeoutInSeconds > 0 ? (int)(blPopRequest.TimeoutInSeconds * 1000) : null
            };

            BlockingManager.Subscribe(blPopRequest.Key.Key, subscribedClient);

            _ = BlockingManager.WaitThenResubmitAsync(subscribedClient, request);

            return new ValueTask<CommandOutcome>(new CommandOutcome.Pending());
        }
    }
}