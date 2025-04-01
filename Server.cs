using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CustomWebServer;

public class Server : IDisposable
{
    private static readonly IPAddress ServerIP = IPAddress.Any;
    private readonly int Port;
    private readonly TcpListener TcpListener;
    private readonly string ProjectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\.."));
    private readonly RequestHandler requestHandler;
    private readonly CancellationTokenSource cancellationTokenSource;
    private readonly int MaxConcurrentConnections = 20;
    private readonly SemaphoreSlim connectionLimiter;
    private bool isRunning;
    private bool disposed;
    public Server(int port = 8080)
    {
        Port = port;
        TcpListener = new TcpListener(ServerIP, Port);
        requestHandler = new(ProjectRoot);
        cancellationTokenSource = new CancellationTokenSource();
        connectionLimiter = new(MaxConcurrentConnections);
        isRunning = false;
    }

    public async Task Start()
    {
        if (isRunning)
        {
            Console.WriteLine("server already running");
            return;
        }
        try
        {
            TcpListener.Start();
            isRunning = true;
            Console.WriteLine($"Server started. Listening on {ServerIP}:{Port}");

            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {

                    await connectionLimiter.WaitAsync(cancellationTokenSource.Token);
                    TcpClient client = await TcpListener.AcceptTcpClientAsync().ConfigureAwait(false);

                    // Set timeout for idle connections
                    client.ReceiveTimeout = 30000; // 30 seconds
                    client.SendTimeout = 30000;    // 30 seconds

                    _ = Task.Run(() => HandleClientAsync(client));
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Operation cancelled");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error accepting client: {ex.Message}");
                }

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error accepting client: {ex.Message}");
        }
        finally
        {
            isRunning = false;
            TcpListener.Stop();
            Console.WriteLine("Server stopped.");
        }
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        try
        {
            string clientIp = ((IPEndPoint)client.Client.RemoteEndPoint!).Address.ToString();
            Console.WriteLine($"Client connected from {clientIp}. Thread ID: {Environment.CurrentManagedThreadId}");

            using NetworkStream stream = client.GetStream();
            // Add timeout
            using CancellationTokenSource cancellationTokenTimeout = new(TimeSpan.FromSeconds(30));
            using StreamReader reader = new(stream, Encoding.UTF8);
            using StreamWriter writer = new(stream, new UTF8Encoding(false)) { AutoFlush = true };

            // Read the HTTP request
            StringBuilder requestBuilder = new();
            string? line;
            Dictionary<string, string> headers = new(StringComparer.OrdinalIgnoreCase);
            string? requestLine = await reader.ReadLineAsync();

            if (string.IsNullOrEmpty(requestLine))
            {
                return; // Empty request
            }

            requestBuilder.AppendLine(requestLine);

            // Parse headers
            while (!string.IsNullOrEmpty(line = await reader.ReadLineAsync()))
            {
                requestBuilder.AppendLine(line);
                var headerParts = line.Split([':'], 2);
                if (headerParts.Length == 2)
                {
                    headers[headerParts[0].Trim()] = headerParts[1].Trim();
                }
            }

            // Read body if Content-Length is present
            if (headers.TryGetValue("Content-Length", out string? contentLengthStr) &&
                int.TryParse(contentLengthStr, out int contentLength) &&
                contentLength > 0)
            {
                char[] buffer = new char[contentLength];
                await reader.ReadBlockAsync(buffer, 0, contentLength);
                requestBuilder.Append(buffer);
            }

            string fullRequest = requestBuilder.ToString();

            // Log the request
            LogRequest(clientIp, fullRequest);

            // Process the request
            HttpContext httpContext = new()
            {
                RequestText = fullRequest,
                Headers = headers,
                ClientIP = clientIp
            };

            HttpResponse response = await requestHandler.HandleRequestAsync(httpContext);

            // Send the response
            await SendResponseAsync(writer, response);

            // Log the response
            LogResponse(clientIp, response);
        }
        catch (IOException ex) when (ex.InnerException is SocketException socketEx &&
                                    (socketEx.SocketErrorCode == SocketError.ConnectionReset ||
                                     socketEx.SocketErrorCode == SocketError.ConnectionAborted))
        {
            Console.WriteLine("Client disconnected unexpectedly.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling client: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
        finally
        {
            client.Close();
            connectionLimiter.Release();
            Console.WriteLine("Client disconnected.");
        }
    }

    private async static Task SendResponseAsync(StreamWriter writer, HttpResponse response)
    {
        StringBuilder responseBuilder = new();

        // Status line
        responseBuilder.AppendLine($"HTTP/1.1 {response.StatusCode}");

        // Headers
        responseBuilder.AppendLine($"Content-Type: {response.ContentType}");
        responseBuilder.AppendLine($"Content-Length: {response.ContentLength}");
        responseBuilder.AppendLine($"Server: CustomWebServer/1.0");
        responseBuilder.AppendLine($"Date: {DateTime.UtcNow.ToString("r")}");

        // Add custom headers
        foreach (var header in response.Headers)
        {
            responseBuilder.AppendLine($"{header.Key}: {header.Value}");
        }

        // Empty line to separate headers from body
        responseBuilder.AppendLine();

        // Send headers
        await writer.WriteAsync(responseBuilder.ToString());

        // Send body
        await writer.WriteAsync(response.Content);
    }

    private static void LogRequest(string clientIp, string request)
    {
        string[] requestLines = request.Split('\n');
        if (requestLines.Length > 0)
        {
            Console.WriteLine($"[{DateTime.Now}] Request from {clientIp}: {requestLines[0].Trim()}");
        }
    }

    private static void LogResponse(string clientIp, HttpResponse response)
    {
        Console.WriteLine($"[{DateTime.Now}] Response to {clientIp}: {response.StatusCode}");
    }

    public void Stop()
    {
        if (!isRunning)
        {
            Console.WriteLine("Server is not running.");
            return;
        }

        Console.WriteLine("Stopping server...");
        cancellationTokenSource.Cancel();
    }

    //Implement IDisposable so we can use the server with 'using' statement
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                Stop();
                cancellationTokenSource.Dispose();
                connectionLimiter.Dispose();
            }

            disposed = true;
        }
    }
}
