using System.Text;

namespace CustomWebServer;

public class HttpResponse
{
    public string StatusCode { get; set; } = HttpStatusCodes.OK;
    public string ContentType { get; set; } = HttpContentTypes.TextHtml;
    public string Content { get; set; } = string.Empty;
    public Dictionary<string, string> Headers { get; set; } = [];
    public int ContentLength => Encoding.UTF8.GetByteCount(Content);
}