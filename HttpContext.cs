namespace CustomWebServer;

public class HttpContext
{
    public string RequestText { get; set; } = string.Empty;
    public Dictionary<string, string> Headers { get; set; } = [];
    public string ClientIP { get; set; } = string.Empty;
    public string Method { get; set; } = null!;
    public string Path { get; set; } = null!;
    public string HttpVersion { get; set; } = string.Empty;
    public string? QueryString { get; set; }
    public Dictionary<string, string> QueryParameters { get; set; } = [];
    public string Body { get; set; } = string.Empty;
}