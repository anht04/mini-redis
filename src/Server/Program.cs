using System.Net;
using System.Net.Sockets;
using System.Text;
using Common;
using Common.Helpers;
using MiniRedis.Helpers;

// You can use print statements as follows for debugging, they'll be visible when running tests.
Console.WriteLine("Logs from your program will appear here!");

Dictionary<string, string> _cache = new();

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
        var request = Encoding.UTF8.GetString(buffer);

        Console.WriteLine($"Request: {request}");

        var parsedArgs = RequestParserHelper.Parse(request);
        var command = parsedArgs[0].ToUpper();
        var response = command switch
        {
            "SET" => _cache.TryAdd(parsedArgs[2], parsedArgs[4])
                ? RESPFormatHelper.FormatSimpleString("OK")
                : RESPFormatHelper.FormatSimpleString("ERR"),
            "GET" => _cache.TryGetValue(parsedArgs[4], out var data)
                ? RESPFormatHelper.FormatBulkString(data)
                : RedisConstants.NullBulkString,
            _ => RESPFormatHelper.FormatErrorString($"ERR unknown command")

        };

        await client.SendAsync(Encoding.UTF8.GetBytes(response));
    }

    client.Close();
}