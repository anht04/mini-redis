using System.Net.Sockets;
using System.Text;
using Common.Helpers;

const string host = "localhost";
const int port = 6379;

using var client = await ConnectWithRetryAsync(host, port, timeoutSeconds: 30);

if (client is null)
{
    Console.WriteLine("Cannot connect to MiniRedis after 10s.");
    return;
}

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

static async Task<TcpClient?> ConnectWithRetryAsync(string host, int port, int timeoutSeconds)
{
    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
    int attempt = 1;

    while (!cts.IsCancellationRequested)
    {
        var client = new TcpClient();
        try
        {
            Console.WriteLine($"[Attempt {attempt}] Connecting to {host}:{port}...");
            
            await client.ConnectAsync(host, port, cts.Token);
            
            return client;
        }
        catch (OperationCanceledException)
        {
            client.Dispose();
            break;
        }
        catch (SocketException ex)
        {
            client.Dispose();
            Console.WriteLine($"   Connect failed ({ex.SocketErrorCode}). Retrying ...");
            
            try
            {
                await Task.Delay(1000, cts.Token);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
        
        attempt++;
    }

    return null;
}