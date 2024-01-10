namespace CustomWebServer.Utilities;

public static class Utils
{
    public static (string method, string path) GetRequestInfo(string? request)
    {
        request ??= "/";
        string[] parts = request.Split(' ');
        if (parts.Length > 2)
        {
            string method = parts[0];
            string path = parts[1];
            return (method, path);
        }
        throw new InvalidOperationException("Invalid HTTP request format");
    }
    public static string GenerateResponse(string statusCode, string contentType, string content)
    {
        return $"HTTP/1.1 {statusCode}\r\nContent-Type: {contentType}\r\n\r\n{content}";
    }  
}
