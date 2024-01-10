namespace CustomWebServer.Routes;

internal class IndexRoute
{
    public static HttpResponse HandleRequest(string projectRoot)
    {
        string htmlPath = Path.Combine(projectRoot, "wwwroot", "temp.html");
        string htmlIndexPage = File.ReadAllText(htmlPath);
        return new HttpResponse("200 OK", "text/html", htmlIndexPage);
    }
}
