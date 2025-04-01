using System.Text.Json;

namespace CustomWebServer.Routes;

internal class TimeRoute
{
    public static Task<HttpResponse> HandleRequestAsync()
    {
        var data = new
        {
            utc = DateTime.UtcNow.ToString("o"),
            local = DateTime.Now.ToString("o"),
            timezone = TimeZoneInfo.Local.DisplayName
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