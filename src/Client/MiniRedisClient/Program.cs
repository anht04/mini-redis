using System.Net.Sockets;
using System.Text;

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

    var request = Encoding.UTF8.GetBytes(input);

    await stream.WriteAsync(request);

    var bytesRead = await stream.ReadAsync(buffer);

    if (bytesRead == 0)
    {
        Console.WriteLine("Server disconnected");
        break;
    }

    var response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

    Console.WriteLine(response);
}