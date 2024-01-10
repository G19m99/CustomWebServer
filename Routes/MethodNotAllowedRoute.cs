namespace CustomWebServer.Routes;

internal class MethodNotAllowedRoute
{
    public static  HttpResponse HandleRequest()
    {
        return new HttpResponse("405 Method Not Allowed", "text/plain", "Only GET requests are allowed");
    }
}
