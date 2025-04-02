using System.Text.Json;

namespace CustomWebServer.Routes;

internal class JsonRoute
{
    public static Task<HttpResponse> HandleRequestAsync(string path)
    {
        var data = new
        {
            message = "Hello from the server!",
            timestamp = DateTime.Now.ToString(),
            version = "1.0"
        };

        string jsonContent = JsonSerializer.Serialize(data, options: new()
        {
            WriteIndented = true
        });

        return Task.FromResult(new HttpResponse
        {
            StatusCode = HttpStatusCodes.OK,
            ContentType = HttpContentTypes.ApplicationJson,
            Content = jsonContent
        });
    }
}
