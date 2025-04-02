namespace CustomWebServer.Utilities;

public static class Constants
{
    public static class HttpStatusCodes
    {
        public const string OK = "200 OK";
        public const string Created = "201 Created";
        public const string Accepted = "202 Accepted";
        public const string NoContent = "204 No Content";

        public const string MovedPermanently = "301 Moved Permanently";
        public const string Found = "302 Found";
        public const string SeeOther = "303 See Other";
        public const string NotModified = "304 Not Modified";

        public const string BadRequest = "400 Bad Request";
        public const string Unauthorized = "401 Unauthorized";
        public const string Forbidden = "403 Forbidden";
        public const string NotFound = "404 Not Found";
        public const string MethodNotAllowed = "405 Method Not Allowed";
        public const string RequestTimeout = "408 Request Timeout";
        public const string Conflict = "409 Conflict";

        public const string InternalServerError = "500 Internal Server Error";
        public const string NotImplemented = "501 Not Implemented";
        public const string BadGateway = "502 Bad Gateway";
        public const string ServiceUnavailable = "503 Service Unavailable";
        public const string GatewayTimeout = "504 Gateway Timeout";
    }

    public static class HttpContentTypes
    {
        public const string TextPlain = "text/plain; charset=UTF-8";
        public const string TextHtml = "text/html; charset=UTF-8";
        public const string ApplicationJson = "application/json; charset=UTF-8";
        public const string ApplicationXml = "application/xml; charset=UTF-8";
        public const string ApplicationFormUrlEncoded = "application/x-www-form-urlencoded";
        public const string ApplicationJavascript = "application/javascript";
        public const string MultipartFormData = "multipart/form-data";
        public const string ApplicationOctetStream = "application/octet-stream";
        public const string ImagePng = "image/png";
        public const string ImageJpeg = "image/jpeg";
        public const string ImageGif = "image/gif";
        public const string ImageSvgXml = "image/svg+xml";
        public const string FontWoff = "font/woff";
        public const string FontWoff2 = "font/woff2";
        public const string FontTtf = "font/ttf";

        public static string GetContentTypeByFileExtension(string extension)
        {
            return extension.ToLowerInvariant() switch
            {
                ".html" => TextHtml,
                ".js" => ApplicationJavascript,
                ".json" => ApplicationJson,
                ".txt" => TextPlain,
                ".png" => ImagePng,
                ".jpg" => ImageJpeg,
                ".jpeg" => ImageJpeg,
                ".gif" => ImageGif,
                ".svg" => ImageSvgXml,
                _ => ApplicationOctetStream,
            };
        }
    }

}
