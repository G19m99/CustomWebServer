using CustomWebServer;

class Program
{
    static async Task Main()
    {
        Server server = new();
        await server.Start();
        Console.ReadLine();
        server.Stop();
    }
}