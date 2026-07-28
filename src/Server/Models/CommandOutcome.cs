namespace MiniRedis.Models
{
    public abstract record CommandOutcome
    {
        public sealed record Completed(string Reply) : CommandOutcome;

        public sealed record Pending : CommandOutcome;
    }
}
