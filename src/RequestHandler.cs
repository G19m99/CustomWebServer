using CustomWebServer.Routes;
using CustomWebServer.Middleware;

namespace CustomWebServer;

public class RequestHandler
{
    private readonly string _filesPath;
    private readonly Dictionary<string, Func<HttpContext, Task<HttpResponse>>> _routes;
    private readonly List<BaseMiddleware> _middlewares = [];

    public RequestHandler(string filesPath)
    {
        _filesPath = filesPath;
        _routes = new Dictionary<string, Func<HttpContext, Task<HttpResponse>>>
        {
            { "GET /", async (context) => await IndexRoute.HandleRequestAsync(_filesPath) },
            { "GET /index.html", async (context) => await IndexRoute.HandleRequestAsync(_filesPath) },
            { "GET /index", async (context) => await IndexRoute.HandleRequestAsync(_filesPath) },
            { "GET /json", async (context) => await JsonRoute.HandleRequestAsync(_filesPath) },
            { "GET /api/time", async (context) => await TimeRoute.HandleRequestAsync() }
        };

        // Add default middlewares
        UseMiddleware(new LoggingMiddleware());
        UseMiddleware(new StaticFilesMiddleware(_filesPath));
    }

    public void UseMiddleware(BaseMiddleware middleware)
    {
        _middlewares.Add(middleware);
    }

    public void AddRoute(string method, string path, Func<HttpContext, Task<HttpResponse>> handler)
    {
        _routes[$"{method.ToUpper()} {path}"] = handler;
    }

    public async Task<HttpResponse> HandleRequestAsync(HttpContext context)
    {
        try
        {
            // Parse request
            ParseRequest(context);

            // Run middlewares
            foreach (var middleware in _middlewares)
            {
                HttpResponse middlewareResponse = await middleware.ProcessAsync(context);
                if (middlewareResponse != null)
                {
                    return middlewareResponse;
                }
            }

            // Check if route exists
            string routeKey = $"{context.Method} {context.Path}";
            if (_routes.TryGetValue(routeKey, out var handler))
            {
                return await handler(context);
            }

            // Check for static file
            if (context.Method == "GET")
            {
                string filePath = Path.Combine(_filesPath, "public", context.Path.TrimStart('/'));
                if (File.Exists(filePath))
                {
                    return await ServeFileAsync(filePath);
                }
            }

            // No route found
            return new HttpResponse
            {
                StatusCode = HttpStatusCodes.NotFound,
                ContentType = HttpContentTypes.TextHtml,
                Content = "<html><body><h1>404 - Page Not Found</h1></body></html>"
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing request: {ex.Message}");
            return new HttpResponse
            {
                StatusCode = HttpStatusCodes.InternalServerError,
                ContentType = HttpContentTypes.TextHtml,
                Content = "<html><body><h1>500 - Internal Server Error</h1></body></html>"
            };
        }
    }

    private static void ParseRequest(HttpContext context)
    {
        string[] requestLines = context.RequestText.Split('\n');
        if (requestLines.Length > 0)
        {
            string[] requestParts = requestLines[0].Split(' ');
            //METHOD PATH VERSION - GET /index HTTP 1.1
            if (requestParts.Length >= 3)
            {
                context.Method = requestParts[0];
                string fullPath = requestParts[1];
                context.HttpVersion = requestParts[2].Trim();

                // Parse path and query string
                string[] pathParts = fullPath.Split('?', 2);
                context.Path = pathParts[0];

                if (pathParts.Length > 1)
                {
                    context.QueryString = pathParts[1];
                    ParseQueryParameters(context);
                }
            }
            else
            {
                throw new InvalidOperationException("Invalid HTTP request format");
            }

            if (requestParts[0] == "POST")
            {
                context.Body = requestLines[^1];
            }
        }
    }

    private static void ParseQueryParameters(HttpContext context)
    {
        if (string.IsNullOrEmpty(context.QueryString))
            return;

        string[] parameters = context.QueryString.Split('&');
        foreach (var param in parameters)
        {
            string[] keyValue = param.Split('=', 2);
            if (keyValue.Length == 2)
            {
                context.QueryParameters[keyValue[0]] = Uri.UnescapeDataString(keyValue[1]);
            }
            else if (keyValue.Length == 1)
            {
                context.QueryParameters[keyValue[0]] = string.Empty;
            }
        }
    }

    private static async Task<HttpResponse> ServeFileAsync(string filePath)
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
        catch (Exception)
        {
            return new HttpResponse
            {
                StatusCode = HttpStatusCodes.NotFound,
                ContentType = HttpContentTypes.TextHtml,
                Content = "<html><body><h1>404 - File Not Found</h1></body></html>"
            };
        }
    }
}
