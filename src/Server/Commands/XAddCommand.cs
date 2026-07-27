using Common.Helpers;
using MiniRedis.Commands.Requests;
using MiniRedis.Data;
using System.Net.Sockets;

namespace MiniRedis.Commands;

public class XAddCommand : ICommand
{
    public int Arity => 5;
    public bool IsWriteCommand => true;

    public async Task<string> ExecuteAsync(List<string> args, RedisDatabase database, Socket client)
    {
        var request = XAddRequest.Create(args);
        var dataId = database.XAdd(request.Key, request.DataId, request.Values);

        return await Task.FromResult(RESPFormatHelper.FormatBulkString(dataId.ToString()));
    }
}