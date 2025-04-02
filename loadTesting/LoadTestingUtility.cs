using CustomWebServer;

namespace LoadTesting;

// WIP
public static class LoadTestingUtility
{
    public static async Task RunSimpleLoadTest()
    {
        CancellationTokenSource ct = new();
        // Start the server
        Server server = new(8080);
        Task serverTask = server.Start(ct.Token);
        HttpClient httpClient = new();

        try
        {
            // Allow time for server to start
            await Task.Delay(500);

            Console.WriteLine("Starting load test - 100 concurrent requests");

            List<Task> tasks = [];

            // Send 100 concurrent requests
            for (int i = 0; i < 100; i++)
            {
                tasks.Add(Task.Run(async () => {
                    try
                    {
                        await httpClient.GetAsync("http://localhost:8080/");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Request failed: {ex.Message}");
                    }
                }));
            }

            // Wait for all requests to complete
            await Task.WhenAll(tasks);
            Console.WriteLine("Load test completed");
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Load test failed: {ex.Message}");
        }
        finally
        {
            ct.Cancel();
            await serverTask;
            server.Dispose();
            httpClient.Dispose();
        }
    }
}