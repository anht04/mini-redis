using System.Net.Sockets;
using System.Text;
using Common;
using Common.Helpers;

using var client = new TcpClient();

await client.ConnectAsync("localhost", 6379);

var stream = client.GetStream();
var buffer = new byte[1024];

Console.WriteLine("Connected to MiniRedis");

while (true)
{
    Console.Write("> ");
    var input = Console.ReadLine();

    if (input is null || input.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    var sendingRequest = RESPFormatHelper.FormatArray(input);

    await stream.WriteAsync(Encoding.UTF8.GetBytes(sendingRequest));

    var bytesRead = await stream.ReadAsync(buffer);

    if (bytesRead == 0)
    {
        Console.WriteLine("Server disconnected");
        break;
    }

    var response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

    Console.WriteLine(response);
}