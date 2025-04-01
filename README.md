# Custom Web Server in C#

A lightweight, extensible web server written in C# that demonstrates core HTTP server functionality and modern .NET practices.


## Coding Challenges 

 - [Write your own Web Server](https://codingchallenges.fyi/challenges/challenge-webserver)


## Features

- **Middleware Pipeline**: Extensible request processing with support for custom middleware
- **Routing System**: Method-based routing with support for dynamic routes and parameters
- **Static File Serving**: Built-in support for serving static files with proper MIME type detection
- **Async/Await Pattern**: Fully asynchronous design for optimal performance
- **Connection Management**: Connection pooling and timeout handling for robustness
- **HTTP Compliance**: Proper status codes, headers, and request/response parsing
- **Error Handling**: Comprehensive error handling and logging throughout

## Getting Started


### Running the Server

```bash
# Run with default settings (port 8080)
dotnet run

# Run with custom port
dotnet run -- 9000
```

The server will start and listen for incoming connections on the specified port (or 8080 by default).

### Project Structure

- `Server.cs`: Core server implementation handling TCP connections
- `RequestHandler.cs`: Processes HTTP requests and manages routes
- `HttpContext.cs`: Holds request information and state
- `HttpResponse.cs`: Represents HTTP responses
- `Middleware`: Contains middleware components for request processing
- `Routes`: Contains route handlers for different endpoints

## Usage Examples

### Adding a Custom Route

```csharp
// Add a simple route
requestHandler.AddRoute("GET", "/hello", async (context) => {
    return new HttpResponse {
        StatusCode = "200 OK",
        ContentType = "text/plain",
        Content = "Hello World!"
    };
});

// Add a route with parameters
requestHandler.AddRoute("GET", "/users", async (context) => {
    string name = context.QueryParameters.GetValueOrDefault("name", "Guest");
    return new HttpResponse {
        StatusCode = "200 OK",
        ContentType = "text/plain",
        Content = $"Hello, {name}!"
    };
});
```

### Creating Custom Middleware

```csharp
public class AuthenticationMiddleware : Middleware
{
    public override Task<HttpResponse> ProcessAsync(HttpContext context)
    {
        // Check for API key
        if (context.Path.StartsWith("/api/") && 
            (!context.Headers.TryGetValue("API-Key", out var apiKey) || apiKey != "secret-key"))
        {
            return Task.FromResult(new HttpResponse {
                StatusCode = "401 Unauthorized",
                ContentType = "text/plain",
                Content = "Invalid or missing API key"
            });
        }
        
        // Continue to next middleware
        return Task.FromResult<HttpResponse>(null);
    }
}

// Register the middleware
requestHandler.UseMiddleware(new AuthenticationMiddleware());
```

## API Documentation

### Server Class

The main server class that handles TCP connections and configures the HTTP pipeline.

```csharp
// Create a server with default settings
var server = new Server();

// Create a server with custom settings
var server = new Server(
    port: 9000,
    projectRoot: "/path/to/files",
    maxConcurrentConnections: 200
);

// Start the server
await server.Start();

// Stop the server
server.Stop();
```

## Testing - working on it

The project includes comprehensive testing utilities:

- **Unit Tests**: Test individual components
- **Integration Tests**: Test the complete request/response cycle
- **Load Tests**: Verify performance under concurrent connections

Run tests using:

```bash
dotnet test
```


