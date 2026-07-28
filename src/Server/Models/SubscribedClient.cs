
namespace MiniRedis.Models
{
    public class SubscribedClient
    {
        public DateTimeOffset SubscribedAt { get; set; }
        public float? TimeoutMilliseconds { get; init; }
        public TaskCompletionSource Singal { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
    }
}
