using System.Net.Sockets;
using Common.Helpers;
using MiniRedis.Commands.Requests;
using MiniRedis.Data;
using MiniRedis.Models;

namespace MiniRedis.Commands;

public class XReadCommand: ICommand
{
    public int Arity => -4;

    public bool IsWriteCommand => false;

    public async Task<string> ExecuteAsync(List<string> args, RedisDatabase database, Socket client)
    {
        var request = XReadRequest.Create(args);
        var currentClient = new SubscribedClient
        {
            Socket = client,
            SubscribedAt = DateTimeOffset.UtcNow,
            SubscribedTo = new TaskCompletionSource<string>(),
            TimeoutInSeconds = request.TimeoutInSeconds
        };

        return RESPFormatHelper.FormatArray(await database.XRead(request, currentClient));
    }
}