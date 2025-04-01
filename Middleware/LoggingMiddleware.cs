namespace CustomWebServer.Middleware;

public class LoggingMiddleware : BaseMiddleware
{
    public override Task<HttpResponse> ProcessAsync(HttpContext context)
    {
        Console.WriteLine($"[{DateTime.Now}] {context.Method} {context.Path} from {context.ClientIP}");
        return Task.FromResult<HttpResponse>(null); // Continue to next middleware
    }
}
