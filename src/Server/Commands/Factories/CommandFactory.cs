using MiniRedis.Constants;

namespace MiniRedis.Commands.Factories;

public static class CommandFactory
{
    private static readonly Dictionary<string, ICommand> Commands = new(StringComparer.OrdinalIgnoreCase)
    {
        { CommandConstants.PING, new PingCommand() },
        { CommandConstants.ECHO, new EchoCommand() },
        { CommandConstants.GET, new GetCommand() },
        { CommandConstants.SET, new SetCommand() },
        { CommandConstants.RPUSH, new RPushCommand() },
        { CommandConstants.LPUSH, new LPushCommand() },
        { CommandConstants.LRANGE, new LRangeCommand() },
        { CommandConstants.LLEN, new LLenCommand() },
        { CommandConstants.LPOP, new LPopCommand() },
        { CommandConstants.BLPOP, new BLPopCommand() },
        { CommandConstants.TYPE, new TypeCommand() },
    };

    public static ICommand? GetCommand(string commandName)
    {
        Commands.TryGetValue(commandName, out var command);
        return command;
    }
}