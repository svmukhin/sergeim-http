# FluentHttp

A fluent HTTP client library for .NET that provides a pure OOP
approach to building and executing HTTP requests with method chaining.

## Installation

```bash
dotnet add package FluentHttp
```

## Quick Start

### Simple GET Request

```csharp
var response = new Request("https://api.example.com")
    .Uri().Path("/users").QueryParam("id", 123).Back()
    .Method(Request.GET)
    .Fetch();
```

### JSON Response

```csharp
string userName = new Request("https://api.example.com")
    .Uri().Path("/users").QueryParam("id", 123).Back()
    .Header(HttpHeaders.ACCEPT, MediaType.APPLICATION_JSON)
    .Fetch()
    .As<JsonResponse>()
    .AssertStatus(200)
    .Json().GetJsonObject().GetString("name");
```

### XML Response with XPath

```csharp
string href = new Request("https://api.example.com")
    .Uri().Path("/data").Back()
    .Header(HttpHeaders.ACCEPT, MediaType.TEXT_XML)
    .Fetch()
    .As<XmlResponse>()
    .AssertStatus(200)
    .AssertXPath("/page/links/link[@rel='see']")
    .EvaluateXPath("/page/links/link[@rel='see']/@href");
```

### Complex Chaining Example

```csharp
string name = new Request("https://www.example.com:8080")
    .Uri().Path("/users").QueryParam("id", 333).Back()
    .Method(Request.GET)
    .Header(HttpHeaders.ACCEPT, MediaType.TEXT_XML)
    .Fetch()
    .As<RestResponse>()
    .AssertStatus(200)
    .As<XmlResponse>()
    .AssertXPath("/page/links/link[@rel='see']")
    .Rel("/page/links/link[@rel='see']/@href")
    .Header(HttpHeaders.ACCEPT, MediaType.APPLICATION_JSON)
    .Fetch()
    .As<JsonResponse>()
    .Json().GetJsonObject().GetString("name");
```

## Wire System

FluentHttp uses a wire system for sending HTTP requests, allowing you to customize and extend request handling through decorators.

### Basic Wire Usage

By default, requests use `HttpWire`:

```csharp
var response = new Request("https://api.example.com").Fetch();
```

### Custom Wire

You can specify a custom wire implementation:

```csharp
var response = new Request("https://api.example.com", new HttpWire()).Fetch();
```

### Changing Wire with Through()

Change the wire at any point using the `Through()` method:

```csharp
var response = new Request("https://api.example.com")
    .Through(new HttpWire())
    .Fetch();
```

### Authorization Wire

Add authorization headers to requests:

```csharp
var response = new Request("https://api.example.com")
    .Through(new BasicAuthWire(new HttpWire(), "Bearer your-token"))
    .Fetch();
```

### Retry Wire

Add automatic retry logic for failed requests:

```csharp
var response = new Request("https://api.example.com", new RetryWire(
    new HttpWire(),
    maxRetries: 5,
    delayBetweenRetries: TimeSpan.FromSeconds(2)
)).Fetch();
```

### Chaining Wire Decorators

Combine multiple decorators for advanced functionality:

```csharp
var wire = new RetryWire(
    new BasicAuthWire(
        new HttpWire(),
        "Bearer token"
    ),
    maxRetries: 3,
    delayBetweenRetries: TimeSpan.FromSeconds(1)
);
var response = new Request("https://api.example.com", wire)
    .Uri().Path("/data").Back()
    .Fetch();
```

Or fluently with `Through()`:

```csharp
var response = new Request("https://api.example.com")
    .Through(new BasicAuthWire(new HttpWire(), "Bearer token"))
    .Uri().Path("/protected-resource").Back()
    .Through(new RetryWire(
        new BasicAuthWire(new HttpWire(), "Bearer token"),
        3,
        TimeSpan.FromSeconds(2)
    ))
    .Fetch();
```

### Custom Wire Implementation

Create your own wire by implementing `IWire`:

```csharp
public class LoggingWire : IWire
{
    private readonly IWire _origin;
    private readonly ILogger _logger;

    public LoggingWire(IWire origin, ILogger logger)
    {
        _origin_ = origin;
        _logger = logger;
    }

    public async Task<HttpResponseMessage> SendAsync(
        string method,
        string uri,
        Dictionary<string, string> headers,
        string? body = null)
    {
        _logger.Log($"Sending {method} request to {uri}");
        var response = await _innerWire.SendAsync(method, uri, headers, body);
        _logger.Log($"Received {response.StatusCode} from {uri}");
        return response;
    }

    public HttpResponseMessage Send(
        string method,
        string uri,
        Dictionary<string, string> headers,
        string? body = null)
    {
        return SendAsync(method, uri, headers, body).GetAwaiter().GetResult();
    }
}
```

## Building

```bash
dotnet build
```

## Testing

```bash
dotnet test
```

## License

See [LICENSE.txt](LICENSE.txt) for details.
