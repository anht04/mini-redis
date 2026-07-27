using System.Net.Sockets;
using MiniRedis.Commands.Requests;
using MiniRedis.Data;
using MiniRedis.Models.GlobalCache;

namespace MiniRedis.Commands;

public class XReadCommand: ICommand
{
    public int Arity => -4;

    public bool IsWriteCommand => false;

    public Task<string> ExecuteAsync(List<string> args, RedisDatabase database, Socket client)
    {
        var request = XReadRequest.Create(args);

        return Task.FromResult(database.XRead(request));
    }
}