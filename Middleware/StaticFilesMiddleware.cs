namespace CustomWebServer.Middleware;

public class StaticFilesMiddleware(string rootPath) : BaseMiddleware
{
    private readonly string rootPath = rootPath;

    public override async Task<HttpResponse> ProcessAsync(HttpContext context)
    {
        if (context.Method == "GET" && !context.Path.StartsWith("/api/"))
        {
            string filePath = Path.Combine(rootPath, "public", context.Path.TrimStart('/'));
            if (File.Exists(filePath))
            {
                string extension = Path.GetExtension(filePath).ToLower();
                string contentType = HttpContentTypes.GetContentTypeByFileExtension(extension);

                try
                {
                    string content = await File.ReadAllTextAsync(filePath);
                    return new HttpResponse
                    {
                        StatusCode = HttpStatusCodes.OK,
                        ContentType = contentType,
                        Content = content
                    };
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error serving static file: {ex.Message}");
                }
            }
        }

        return null; // Continue to next middleware
    }
}
