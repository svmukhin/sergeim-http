# SergeiM.Http

![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/svmukhin/sergeim-http/build.yml)
![NuGet Version](https://img.shields.io/nuget/v/SergeiM.Http?color=%230000FF)
[![Hits-of-Code](https://hitsofcode.com/github/svmukhin/sergeim-http)](https://hitsofcode.com/github/svmukhin/sergeim-http/view)
![GitHub License](https://img.shields.io/github/license/svmukhin/sergeim-http)

A fluent HTTP client library for .NET that provides a pure OOP
approach to building and executing HTTP requests with method chaining.

## Installation

```bash
dotnet add package SergeiM.Http
```

## Quick Start

### Simple GET Request

```csharp
var response = await new BaseRequest("https://api.example.com")
    .Uri().Path("/users").QueryParam("id", 123).Back()
    .Method(BaseRequest.GET)
    .FetchAsync();
```

### JSON Response

```csharp
var response = await new BaseRequest("https://api.example.com")
    .Uri().Path("/users").QueryParam("id", 123).Back()
    .Header(HttpHeaders.ACCEPT, MediaType.APPLICATION_JSON)
    .FetchAsync();

var json = response.As<JsonResponse>().AssertStatus(200);
var user = json.AsObject();
string userName = user.GetString("name");
int age = user.GetInt("age");
bool isActive = user.GetBoolean("is_active", false);
```

### XML Response with XPath

```csharp
var response = await new BaseRequest("https://api.example.com")
    .Uri().Path("/data").Back()
    .Header(HttpHeaders.ACCEPT, MediaType.TEXT_XML)
    .FetchAsync();

string href = response.As<XmlResponse>()
    .AssertStatus(200)
    .AssertXPath("/page/links/link[@rel='see']")
    .EvaluateXPath("/page/links/link[@rel='see']/@href");
```

### Complex Chaining Example

```csharp
var firstResponse = await new BaseRequest("https://www.example.com:8080")
    .Uri().Path("/users").QueryParam("id", 333).Back()
    .Method(BaseRequest.GET)
    .Header(HttpHeaders.ACCEPT, MediaType.TEXT_XML)
    .FetchAsync();

var secondResponse = await firstResponse
    .As<XmlResponse>()
    .AssertStatus(200)
    .AssertXPath("/page/links/link[@rel='see']")
    .Rel("/page/links/link[@rel='see']/@href")
    .Header(HttpHeaders.ACCEPT, MediaType.APPLICATION_JSON)
    .FetchAsync();
```

## Synchronous Methods (Blocks Threads)

The library also exposes synchronous wrappers for backward compatibility and scripting
scenarios. These methods call their async counterparts via `.GetAwaiter().GetResult()`,
which blocks the calling thread and can lead to deadlocks in UI or ASP.NET contexts.

| Sync method / property | Async alternative       | Blocks on             |
| ---------------------- | ----------------------- | --------------------- |
| `IRequest.Fetch()`     | `IRequest.FetchAsync()` | `FetchAsync()`        |
| `IWire.Send()`         | `IWire.SendAsync()`     | `SendAsync()`         |
| `IResponse.Content`    | — (always blocking)     | `ReadAsStringAsync()` |

**Prefer `FetchAsync()` and `SendAsync()` in all new code.**

### Synchronous usage (not recommended)

```csharp
var response = new BaseRequest("https://api.example.com")
    .Uri().Path("/users").QueryParam("id", 123).Back()
    .Method(BaseRequest.GET)
    .Fetch();
```

## HttpClient Lifecycle

The default `new HttpWire()` constructor creates `new HttpClient()` and holds
it for the wire's lifetime. This is fine for short-lived apps but causes
**socket exhaustion** and **stale DNS** in long-running server applications.

### Console apps / one-shot scripts

For short-lived processes, a single `HttpClient` instance is all you need:

```csharp
using var client = new HttpClient { BaseAddress = new Uri("https://api.example.com") };
var wire = new HttpWire(client);
var user = await new BaseRequest("/users/1", wire).FetchAsync();
```

### ASP.NET Core / long-running services

Use `IHttpClientFactory` via `ManagedHttpWire` — it creates and disposes
an `HttpClient` per request, letting the factory manage handler pooling and DNS
rotation:

```csharp
// Program.cs
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IWire>(sp =>
    new ManagedHttpWire(() => sp.GetRequiredService<IHttpClientFactory>().CreateClient()));
```

With a named client for custom settings:

```csharp
builder.Services.AddHttpClient("github", c =>
{
    c.BaseAddress = new Uri("https://api.github.com");
    c.DefaultRequestHeaders.Add("User-Agent", "MyApp");
});
builder.Services.AddSingleton<IWire>(sp =>
    new ManagedHttpWire(() => sp.GetRequiredService<IHttpClientFactory>().CreateClient("github")));
```

`ManagedHttpWire` is stateless and can be registered as a **Singleton**.

> **Warning**: Avoid `new HttpWire()` (parameterless) in server applications.
> It creates an unmanaged `HttpClient` that will exhaust sockets over time.

## Wire System

SergeiM.Http uses a wire system for sending HTTP requests, allowing
you to customize and extend request handling through decorators.

### Basic Wire Usage

By default, requests use `HttpWire` internally. For long-running applications,
prefer `ManagedHttpWire` with `IHttpClientFactory` (see [HttpClient Lifecycle](#httpclient-lifecycle)).

```csharp
var response = await new BaseRequest("https://api.example.com").FetchAsync();
```

### Custom Wire

You can specify a custom wire implementation:

```csharp
var response = await new BaseRequest("https://api.example.com",
    new ManagedHttpWire(() => new HttpClient())).FetchAsync();
```

### Changing Wire with Through()

Change the wire at any point using the `Through()` method:

```csharp
var response = await new BaseRequest("https://api.example.com")
    .Through(new ManagedHttpWire(() => new HttpClient()))
    .FetchAsync();
```

### Authorization Wire

Add authorization headers to requests:

```csharp
var response = await new BaseRequest("https://api.example.com")
    .Through(new BasicAuthWire(
        new ManagedHttpWire(() => new HttpClient()),
        "Bearer your-token"))
    .FetchAsync();
```

### Retry Wire

Add automatic retry logic for failed requests:

```csharp
var response = await new BaseRequest("https://api.example.com", new RetryWire(
    new ManagedHttpWire(() => new HttpClient()),
    maxRetries: 5,
    delayBetweenRetries: TimeSpan.FromSeconds(2)
)).FetchAsync();
```

### Chaining Wire Decorators

Combine multiple decorators for advanced functionality:

```csharp
var wire = new RetryWire(
    new BasicAuthWire(
        new ManagedHttpWire(() => new HttpClient()),
        "Bearer token"
    ),
    maxRetries: 3,
    delayBetweenRetries: TimeSpan.FromSeconds(1)
);
var response = await new BaseRequest("https://api.example.com", wire)
    .Uri().Path("/data").Back()
    .FetchAsync();
```

Or fluently with `Through()`:

```csharp
var response = await new BaseRequest("https://api.example.com")
    .Through(new BasicAuthWire(
        new ManagedHttpWire(() => new HttpClient()),
        "Bearer token"))
    .Uri().Path("/protected-resource").Back()
    .Through(new RetryWire(
        new BasicAuthWire(
            new ManagedHttpWire(() => new HttpClient()),
            "Bearer token"),
        3,
        TimeSpan.FromSeconds(2)
    ))
    .FetchAsync();
```

### Custom Wire Implementation

Create your own wire by implementing `IWire`. Always implement `SendAsync()` as
the primary method. The synchronous `Send()` method is provided for backward
compatibility — it blocks the calling thread and should be avoided in new code.

```csharp
public class LoggingWire : IWire
{
    private readonly IWire _origin;
    private readonly ILogger _logger;

    public LoggingWire(IWire origin, ILogger logger)
    {
        _origin = origin;
        _logger = logger;
    }

    public async Task<HttpResponseMessage> SendAsync(
        string method,
        string uri,
        Dictionary<string, string> headers,
        string? body = null)
    {
        _logger.Log($"Sending {method} request to {uri}");
        var response = await _origin.SendAsync(method, uri, headers, body);
        _logger.Log($"Received {response.StatusCode} from {uri}");
        return response;
    }

    // Provided for sync consumers only — blocks the calling thread.
    // Prefer SendAsync() in new code.
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

## Conventional Commits

This project follows [Conventional Commits](https://www.conventionalcommits.org/)
to automate versioning and changelog generation via
[release-please](https://github.com/googleapis/release-please).

| Type       | Purpose                             | Bump  |
| ---------- | ----------------------------------- | ----- |
| `feat`     | New feature                         | minor |
| `fix`      | Bug fix                             | patch |
| `docs`     | Documentation only changes          | —     |
| `style`    | Code style (formatting, whitespace) | —     |
| `refactor` | Code refactoring                    | —     |
| `test`     | Adding or updating tests            | —     |
| `chore`    | Maintenance (CI, deps, etc.)        | —     |

Breaking changes are signaled with `!` after the type (`feat!:`)
or a `BREAKING CHANGE:` footer — triggers a major bump.

## License

See [LICENSE.txt](LICENSE.txt) for details.
