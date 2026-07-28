using MiniRedis.Commands.Requests;
using MiniRedis.Data;
using MiniRedis.Models;

namespace MiniRedis.Commands.Abstractions
{
    public interface ICommand
    {
        int Arity { get; }
        bool IsWriteCommand { get; }
        ValueTask<CommandOutcome> TryExecuteAsync(CommandRequest request, RedisDatabase database);
    }
}
