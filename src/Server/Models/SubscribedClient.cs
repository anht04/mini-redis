using System.Net.Sockets;

namespace MiniRedis.Models
{
    public class SubscribedClient
    {
        public Socket Socket { get; set; }
        public TaskCompletionSource<string> SubscribedTo { get; set; }
        public DateTimeOffset SubscribedAt { get; set; }
        public int TimeoutInSeconds { get; set; }
        public bool IsExpired => (DateTimeOffset.UtcNow - SubscribedAt).TotalSeconds > TimeoutInSeconds;
    }
}
