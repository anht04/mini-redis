using Common.Helpers;
using MiniRedis.Commands.Requests;
using MiniRedis.Data;
using System.Net.Sockets;

namespace MiniRedis.Commands;

public class TypeCommand : ICommand
{
    public int Arity => -3;
    public bool IsWriteCommand => false;
    public Task<string> ExecuteAsync(List<string> args, RedisDatabase database, Socket client)
    {
        var request = SingleKeyRequest.Create(args);

        return Task.FromResult(RESPFormatHelper.FormatSimpleString(database.Type(request.Key)));
    }
}