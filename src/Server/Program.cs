using System.Net;
using System.Net.Sockets;
using System.Text;
using Common.Helpers;
using MiniRedis.Commands.Factories;
using MiniRedis.Helpers;
using MiniRedis.Models.GlobalCache;

// You can use print statements as follows for debugging, they'll be visible when running tests.
Console.WriteLine("Logs from your program will appear here!");

Dictionary<RedisEntry, RedisValue> _cache = new();

var server = new TcpListener(IPAddress.Any, 6379);
server.Start();

while (true)
{
    var client = await server.AcceptSocketAsync();
    _ = HandleClientAsync(client);
}

async Task HandleClientAsync(Socket client)
{
    var buffer = new byte[1024];
    while (true)
    {
        var byteReads = await client.ReceiveAsync(buffer);

        if (byteReads == 0)
        {
            break;
        }
        var request = Encoding.UTF8.GetString(buffer, 0 , byteReads);

        // Console.WriteLine($"Request: {request}");

        var parsedArgs = RequestParserHelper.Parse(request);
        string response;
        var commandName = parsedArgs[0].ToUpper();
        var command = CommandFactory.GetCommand(commandName);
        if (command != null)
        {
            try
            {
                response = await command.ExecuteAsync(parsedArgs, _cache, client);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception occured:" + e);
                throw;
            }
        }
        else
        {
            response = RESPFormatHelper.FormatSimpleErrorString("Unknown command: " + commandName);
        }

        await client.SendAsync(Encoding.UTF8.GetBytes(response));
    }

    client.Close();
}