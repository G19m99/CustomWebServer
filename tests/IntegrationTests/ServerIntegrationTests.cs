using System.Net;
using System.Text;
using System.Text.Json;
using static CustomWebServer.Utilities.Constants;

namespace CustomWebServer.Tests.IntegrationTests;

public class ServerIntegrationTests : IDisposable
{
    private const int PORT = 8090;// Special testing port
    private const string BASE_URL = "http://localhost:8090";
    private readonly Server _server;
    private readonly Task _serverTask;
    private readonly HttpClient _httpClient;

    public ServerIntegrationTests()
    {
        CancellationTokenSource cts = new();
        _server = new Server(PORT);
        _serverTask = _server.Start(cts.Token);

        // potentially need to wait for server startup

        _httpClient = new()
        {
            Timeout = TimeSpan.FromSeconds(10)
        };
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        _server.Stop();
    }

    [Fact]
    public async Task TestRootEndpoint()
    {
        // Act
        HttpResponseMessage response = await _httpClient.GetAsync($"{BASE_URL}/");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);

        //get body and assert
        string body = await response.Content.ReadAsStringAsync();
        Assert.Contains("<html>", body);
    }

    [Fact]
    public async Task TestJsonEndpoint()
    {
        // Act
        HttpResponseMessage response = await _httpClient.GetAsync($"{BASE_URL}/json");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

        string body = await response.Content.ReadAsStringAsync();
        JsonDocument jsonDoc = JsonDocument.Parse(body);
        Assert.True(jsonDoc.RootElement.TryGetProperty("message", out var _));
    }

    [Fact]
    public async Task TestTimeRoute()
    {
        // Act
        HttpResponseMessage response = await _httpClient.GetAsync($"{BASE_URL}/api/time");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

        string body = await response.Content.ReadAsStringAsync();
        JsonDocument jsonDoc = JsonDocument.Parse(body);
        Assert.True(jsonDoc.RootElement.TryGetProperty("utc", out var _));
    }

    [Fact]
    public async Task TestNonExistentRoute()
    {
        // Act
        HttpResponseMessage response = await _httpClient.GetAsync($"{BASE_URL}/non-existent");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task TestStaticFile()
    {
        // Arrange
        string tempDir = Path.Combine("C:\\Dev\\CustomWebServer.Tests\\public");
        Directory.CreateDirectory(tempDir);

        string filePath = Path.Combine(tempDir, "tests.html");
        File.WriteAllText(filePath, "<html><body><h1>Test Static File</h1></body></html>");

        // Act
        HttpResponseMessage response = await _httpClient.GetAsync($"{BASE_URL}/tests.html");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);

        string body = await response.Content.ReadAsStringAsync();
        Assert.Contains("<html>", body);
    }

    [Fact]
    public async Task TestQueryParameters()
    {
        // Arrange
        _server.RequestHandler.AddRoute("GET", "/query-strings", async (context) =>
        {
            return new HttpResponse()
            {
                StatusCode = HttpStatusCodes.OK,
                ContentType = HttpContentTypes.ApplicationJson,
                Content = JsonSerializer.Serialize(context.QueryParameters)
            };
        });

        // Act
        HttpResponseMessage response = await _httpClient.GetAsync($"{BASE_URL}/query-strings?name=Hello&value=World!");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        string body = await response.Content.ReadAsStringAsync();
        JsonDocument jsonDoc = JsonDocument.Parse(body);
        Assert.True(jsonDoc.RootElement.TryGetProperty("name", out var nameProperty));
        Assert.True(jsonDoc.RootElement.TryGetProperty("value", out var valueProperty));

        string greeting = $"{nameProperty} {valueProperty}";
        Assert.Equal("Hello World!", greeting);
    }

    [Fact]
    public async Task TestPostRequest()
    {
        // Arrange
        _server.RequestHandler.AddRoute("POST", "/submit-order", async (context) =>
        {
            return new HttpResponse()
            {
                StatusCode = HttpStatusCodes.Created,
                ContentType = HttpContentTypes.TextPlain,
                Content = $"Received order: {context.Body}"
            };
        });

        // Act
        StringContent orderContent = new("New PC", Encoding.UTF8, "text/plain");
        HttpResponseMessage response = await _httpClient.PostAsync($"{BASE_URL}/submit-order", orderContent);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("text/plain", response.Content.Headers.ContentType?.MediaType);

        string body = await response.Content.ReadAsStringAsync();
        Assert.Contains("New PC", body);
    }
}
