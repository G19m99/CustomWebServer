using System.Net;
using System.Net.Sockets;
using static CustomWebServer.Utilities.Utils;

namespace CustomWebServer;

public class Server
{
    private static readonly IPAddress ServerIP = IPAddress.Any;
    private const int Port = 8080;
    private readonly TcpListener TcpListener;
    private readonly string ProjectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\.."));
    private readonly RequestHandler requestHandler;
    public Server()
    {
        TcpListener = new TcpListener(ServerIP, Port);
        requestHandler = new(ProjectRoot);
    }

    public async Task Start()
    {
        try
        {
            TcpListener.Start();
            Console.WriteLine($"Server started. Listening on {ServerIP}:{Port}");

            while (true)
            {
                TcpClient client = await TcpListener.AcceptTcpClientAsync().ConfigureAwait(false);
                _ = Task.Run(() => HandleClient(client));

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error accepting client: {ex.Message}");
        }
    }

    private async Task HandleClient(TcpClient client)
    {
        try
        {
            Console.WriteLine($"Connected! Current thread ID: {Environment.CurrentManagedThreadId}");
            using NetworkStream stream = client.GetStream();
            using StreamReader reader = new(stream);
            using StreamWriter writer = new(stream) { AutoFlush = true };

            string? req = await reader.ReadLineAsync();

            HttpResponse rs = requestHandler.HandleRequest(req ?? string.Empty);
            string response = GenerateResponse(rs.StatusCode, rs.ContentType, rs.Content);
            await writer.WriteAsync(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling client: {ex.Message}");
        }
        finally
        {
            client.Close();
            Console.WriteLine("Client disconnected.");
        }
    }

    public void Stop()
    {
        TcpListener.Stop();
        Console.WriteLine("Server closed");
    }
}
