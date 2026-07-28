using System.Threading.Channels;

namespace MiniRedis.Commands.Requests
{
    public record CommandRequest
    {
        public required List<string> Args { get; init; }
        public required ICommand Command { get; init; }
        public required ChannelWriter<CommandRequest> Writer { get; init; }
        public TaskCompletionSource<string> ReplyTcs { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public bool IsRetry { get; set; } = false;
        public bool IsTimedOut { get; set; } = false;
    }
}
