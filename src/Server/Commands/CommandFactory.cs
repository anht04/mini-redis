using MiniRedis.Commands.Abstractions;

namespace MiniRedis.Commands;

public static class CommandFactory
{
    private static readonly Dictionary<string, ICommand> _commands = new(StringComparer.OrdinalIgnoreCase)
    {
        { "PING", new PingCommand() },
        { "ECHO", new EchoCommand() },
        { "GET", new GetCommand() },
        { "SET", new SetCommand() },
    };

    public static ICommand? GetCommand(string commandName)
    {
        _commands.TryGetValue(commandName, out var command);
        return command;
    }
}