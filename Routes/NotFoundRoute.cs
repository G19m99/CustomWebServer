namespace CustomWebServer.Routes;

internal class NotFoundRoute
{
    public static HttpResponse HandleRequest()
    {
        return new HttpResponse("400 Not Found", "text/plain", "Not Found");
    }
}
