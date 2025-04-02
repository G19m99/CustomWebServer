namespace CustomWebServer.Middleware;

public abstract class BaseMiddleware
{
    public abstract Task<HttpResponse> ProcessAsync(HttpContext context);
}