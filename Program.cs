global using static CustomWebServer.Utilities.Constants;
using CustomWebServer;

class Program
{
    static async Task Main(string[] args)
    {
        int port = 8080;

        // Check if port is provided as command line argument
        if (args.Length > 0 && int.TryParse(args[0], out int customPort))
        {
            port = customPort;
        }

        using Server server = new(port);

        // Setup cancellation token to handle Ctrl+C
        CancellationTokenSource cts = new();
        Console.CancelKeyPress += (s, e) =>
        {
            Console.WriteLine("Shutting down server...");
            cts.Cancel();
            e.Cancel = true;
        };

        // Run the server
        Task serverTask = server.Start();

        try
        {
            await serverTask;
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Server shutdown requested.");
        }

        Console.WriteLine("Server has stopped. Press any key to exit.");
        Console.ReadKey();
    }
}