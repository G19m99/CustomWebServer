namespace CustomWebServer.Routes;

internal class JsonRoute
{
    public static HttpResponse HandleRequest(string path)
    {
        var jsonContent = "{\"message\": \"Hello, JSON!\", \"path\": \"" + path + "\"}";
        return new HttpResponse("200 OK", "text/json", jsonContent);
    }
}
