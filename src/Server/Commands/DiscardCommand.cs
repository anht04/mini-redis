using MiniRedis.Commands.Requests;
using MiniRedis.Data;
using MiniRedis.Models;

namespace MiniRedis.Commands
{
    public class DiscardCommand : ICommand
    {
        public int Arity => throw new NotImplementedException();

        public bool IsWriteCommand => throw new NotImplementedException();

        public ValueTask<CommandOutcome> TryExecuteAsync(CommandRequest request, RedisDatabase database)
        {
            throw new NotImplementedException();
        }
    }
}
