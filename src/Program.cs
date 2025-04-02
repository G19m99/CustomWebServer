global using static CustomWebServer.Utilities.Constants;
using CustomWebServer;

class Program
{
    /// <summary>
    /// User defined routes can be placed here
    /// </summary>
    public static void ConfigureRoutes(RequestHandler requestHandler)
    {
        // Sample added route with query string
        requestHandler.AddRoute("GET", "/hello", async (context) =>
        {
            string name = context.QueryParameters.GetValueOrDefault("name", "Guest");
            return new HttpResponse
            {
                StatusCode = "200 OK",
                ContentType = "text/plain",
                Content = $"Hello {name}"
            };
        });

        // More routes can be added here...
    }

    static async Task Main(string[] args)
    {
        int port = 8080;

        // Check if port is provided as command line argument
        if (args.Length > 0 && int.TryParse(args[0], out int customPort))
        {
            port = customPort;
        }

        using Server server = new(port);

        // Register custom routes
        ConfigureRoutes(server.RequestHandler);

        // Setup cancellation token to handle server shut down
        CancellationTokenSource cts = new();
        Console.CancelKeyPress += (s, e) =>
        {
            Console.WriteLine("Server shutdown requested.");
            e.Cancel = true;
            cts.Cancel();
        };

        // Run the server
        Task serverTask = server.Start(cts.Token);

        try
        {
            await serverTask;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception: {0}", ex.Message);
        }

        Console.WriteLine("Server stopped. Press any key to exit.");
        Console.ReadKey();
    }
}