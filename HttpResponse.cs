namespace CustomWebServer;

public class HttpResponse(string statusCode, string contentType, string content)
{
    public string StatusCode { get; } = statusCode ?? throw new ArgumentNullException(nameof(statusCode));
    public string ContentType { get; } = contentType ?? throw new ArgumentNullException(nameof(contentType));
    public string Content { get; } = content ?? throw new ArgumentNullException(nameof(content));
}