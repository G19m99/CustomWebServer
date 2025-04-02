namespace CustomWebServer.Routes;

internal class MethodNotAllowedRoute
{
    public static  HttpResponse HandleRequest()
    {
        return new HttpResponse
        {
            StatusCode = HttpStatusCodes.MethodNotAllowed,
            ContentType = HttpContentTypes.ApplicationJson,
            Content = "Not Allowed"
        };
    }
}
