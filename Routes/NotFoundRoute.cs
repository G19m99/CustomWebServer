namespace CustomWebServer.Routes;

internal class NotFoundRoute
{
    public static HttpResponse HandleRequest()
    {
        return new()
        {
            StatusCode = HttpStatusCodes.NotFound,
            ContentType = HttpContentTypes.TextPlain,
            Content = "Not Found"
        };
    }
}
