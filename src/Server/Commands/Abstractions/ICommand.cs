namespace MiniRedis.Commands
{
    public interface ICommand
    {
        string Execute(List<string> args, Dictionary<string, string> cache);
    }
}
