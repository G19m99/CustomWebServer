using CustomWebServer.Routes;
using static CustomWebServer.Utilities.Utils;

namespace CustomWebServer;

internal class RequestHandler(string filesPath)
{
    private readonly string FilesPath = filesPath;

    public HttpResponse HandleRequest(string requestPath)
    {
        return GetPathFromRequest(requestPath) switch
        {
            "/" or "/index.html" or "/index" => IndexRoute.HandleRequest(FilesPath),
            "/json" => JsonRoute.HandleRequest(FilesPath),
            _ => NotFoundRoute.HandleRequest(),
        };
    }
}
