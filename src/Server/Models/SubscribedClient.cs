using System.Net.Sockets;

namespace MiniRedis.Models
{
    public class SubscribedClient
    {
        public Socket Socket { get; set; }
        public TaskCompletionSource<string> SubscribedTo { get; init; }
        public DateTimeOffset SubscribedAt { get; set; }
        public float? TimeoutInSeconds { get; init; }
        public bool IsExpired => TimeoutInSeconds > 0 && 
                                 (DateTimeOffset.UtcNow - SubscribedAt).TotalSeconds > TimeoutInSeconds;
    }
}
