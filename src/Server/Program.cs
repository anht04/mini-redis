using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Channels;
using Common.Constants;
using Common.Helpers;
using MiniRedis.Commands.Factories;
using MiniRedis.Commands.Requests;
using MiniRedis.Data;
using MiniRedis.Helpers;
using MiniRedis.Models;

// You can use print statements as follows for debugging, they'll be visible when running tests.
Console.WriteLine("Logs from your program will appear here!");

RedisDatabase database = new RedisDatabase();

var channel = Channel.CreateUnbounded<CommandRequest>(new UnboundedChannelOptions
{
    SingleReader = true
});

_ = ProcessCommandLoopAsync(channel.Reader, database, channel.Writer);

var server = new TcpListener(IPAddress.Any, 6379);
server.Start();

while (true)
{
    var client = await server.AcceptSocketAsync();
    _ = HandleClientAsync(client, channel.Writer);
}

async Task HandleClientAsync(Socket client, ChannelWriter<CommandRequest> writer)
{
    bool isInTransaction = false;
    var queuedArgs = new Queue<List<string>>();

    var buffer = new byte[1024];
    while (true)
    {
        var byteReads = await client.ReceiveAsync(buffer);

        if (byteReads == 0)
        {
            break;
        }
        var rawRequest = Encoding.UTF8.GetString(buffer, 0, byteReads);

        // Console.WriteLine($"Request: {request}");

        var parsedArgs = RequestParserHelper.Parse(rawRequest);
        string response;
        var commandName = parsedArgs[0].ToUpper();
        var command = CommandFactory.GetCommand(commandName);

        if (command != null)
        {
            if (commandName == "MULTI")
            {
                isInTransaction = true;
                response = RESPFormatHelper.FormatSimpleString("OK");
            }

            else if(isInTransaction && commandName != "EXEC")
            {
                queuedArgs.Enqueue(parsedArgs);
                response = RESPFormatHelper.FormatSimpleString("QUEUED");
            }

            else if (commandName == "EXEC")
            {
                if (!isInTransaction)
                {
                    response = RESPFormatHelper.FormatSimpleErrorString(RedisErrorMessages.Transaction.ExecWithoutMulti);
                }
                else
                {
                    response = RESPFormatHelper.FormatSimpleString(RedisErrorMessages.Transaction.ArgsNotFound);
                    while(queuedArgs.Count > 0)
                    {
                        var nextParsedArgs = queuedArgs.Dequeue();
                        var nextCommandName = parsedArgs[0].ToUpper();
                        var nextCommand = CommandFactory.GetCommand(commandName);
                        var request = new CommandRequest { Args = queuedArgs.Dequeue(), Command = nextCommand!, Writer = writer };
                        try
                        {
                            await writer.WriteAsync(request);
                            response = await request.ReplyTcs.Task;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Exception occured:" + e);
                            response = RESPFormatHelper.FormatSimpleErrorString(e.Message);
                        }
                        isInTransaction = false;
                    }
                }
            }

            else
            {
                var request = new CommandRequest { Args = parsedArgs, Command = command, Writer = writer };
                try
                {
                    await writer.WriteAsync(request);
                    response = await request.ReplyTcs.Task;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception occured:" + e);
                    response = RESPFormatHelper.FormatSimpleErrorString(e.Message);
                }
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

static async Task ProcessCommandLoopAsync(ChannelReader<CommandRequest> reader,
    RedisDatabase database,
    ChannelWriter<CommandRequest> writer)
{
    await foreach (var request in reader.ReadAllAsync())
    {
        try
        {
            var outcome = await request.Command.TryExecuteAsync(request, database);
            if (outcome is CommandOutcome.Completed completed)
            {
                request.ReplyTcs.TrySetResult(completed.Reply);
            }
        }
        catch (Exception ex)
        {
            request.ReplyTcs.TrySetResult(RESPFormatHelper.FormatSimpleErrorString(ex.Message));
        }
    }
}