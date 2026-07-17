using MiniRedis.Models;

namespace MiniRedis.Commands.AsyncManagers
{
    public static class BlockingManager
    {
        private static readonly Dictionary<string, Queue<SubscribedClient>> _blockedClients = [];
        public static void Subscribe(string key, SubscribedClient client)
        {
            client.SubscribedAt = DateTimeOffset.UtcNow;

            if (!_blockedClients.TryGetValue(key, out var subscribedClients))
            {
                subscribedClients = new Queue<SubscribedClient>();
                _blockedClients.Add(key, subscribedClients);
            }
            subscribedClients.Enqueue(client);
        }

        public static SubscribedClient? GetLongestClient(string key)
        {
            if (!_blockedClients.TryGetValue(key, out var subscribedClients))
            {
                return null;
            }

            while (subscribedClients.Count > 0)
            {
                var longestClient = subscribedClients.Dequeue();
                if (!longestClient.IsExpired)
                {
                    return longestClient;
                }
            }

            return null;
        }
    }
}
