using CustomWebServer.Routes;
using static CustomWebServer.Utilities.Utils;

namespace CustomWebServer;

internal class RequestHandler(string filesPath)
{
    private readonly string FilesPath = filesPath;

    public HttpResponse HandleRequest(string requestPath)
    {
        var (method, info) = GetRequestInfo(requestPath);
        Console.WriteLine($"the method was {method} and info {info}");
        if (method != "GET") return MethodNotAllowedRoute.HandleRequest();
        return info switch
        {
            "/" or "/index.html" or "/index" => IndexRoute.HandleRequest(FilesPath),
            "/json" => JsonRoute.HandleRequest(FilesPath),
            _ => NotFoundRoute.HandleRequest(),
        };
    }
}
