namespace CustomWebServer.Utilities;

public static class Utils
{
    public static string GetPathFromRequest(string? request)
    {
        string[] parts = request!.Split(' ');
        if (parts.Length > 1)
        {
            return parts[1];
        }
        return "/";
    }
    public static string GenerateResponse(string statusCode, string contentType, string content)
    {
        return $"HTTP/1.1 {statusCode}\r\nContent-Type: {contentType}\r\n\r\n{content}";
    }  
}
