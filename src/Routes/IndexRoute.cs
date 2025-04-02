namespace CustomWebServer.Routes;

internal class IndexRoute
{
    public static Task<HttpResponse> HandleRequestAsync(string rootPath)
    {
        string filePath = Path.Combine(rootPath, "wwwroot", "temp.html");
        string content;

        try
        {
            content = File.ReadAllText(filePath);
        }
        catch (Exception)
        {
            //TODO: return a different response if not found with 404 status code
            content = "<html><body><h1>Welcome to My Web Server</h1><p>Default page content.</p></body></html>";
        }

        return Task.FromResult(new HttpResponse
        {
            StatusCode = HttpStatusCodes.OK,
            ContentType = HttpContentTypes.TextHtml,
            Content = content
        });

    }
}
