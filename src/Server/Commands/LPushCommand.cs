using System.Net.Sockets;
using Common.Helpers;
using MiniRedis.Commands.Requests;
using MiniRedis.Data;

namespace MiniRedis.Commands
{
    public class LPushCommand : ICommand
    {
        public int Arity => -3;

        public bool IsWriteCommand => true;

        public Task<string> ExecuteAsync(List<string> args, RedisDatabase database, Socket client)
        {
            var request = PushRequest.Create(args);

            return Task.FromResult(RESPFormatHelper.FormatInteger(database.LPush(request.Key, request.Values)));
        }
    }
}