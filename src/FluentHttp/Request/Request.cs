namespace FluentHttp;

/// <summary>
/// Fluent HTTP request builder.
/// </summary>
public class Request : IRequest
{
    public const string GET = "GET";
    public const string POST = "POST";
    public const string PUT = "PUT";
    public const string DELETE = "DELETE";
    public const string PATCH = "PATCH";
    public const string HEAD = "HEAD";
    public const string OPTIONS = "OPTIONS";

    private readonly string _baseUri;
    private UriBuilder? _uriBuilder;
    private string _method = GET;
    private readonly Dictionary<string, string> _headers;
    private string? _body;

    /// <summary>
    /// Creates a new HTTP request with the specified base URI.
    /// </summary>
    public Request(string uri)
    {
        _baseUri = uri;
        _headers = [];
    }

    /// <summary>
    /// Gets the URI builder for constructing the request URI.
    /// </summary>
    public UriBuilder Uri()
    {
        _uriBuilder ??= new UriBuilder(this, _baseUri);
        return _uriBuilder;
    }

    /// <summary>
    /// Sets the HTTP method for the request.
    /// </summary>
    public Request Method(string method)
    {
        _method = method;
        return this;
    }

    /// <summary>
    /// Adds a header to the request.
    /// </summary>
    public Request Header(string name, string value)
    {
        _headers[name] = value;
        return this;
    }

    /// <summary>
    /// Sets the request body.
    /// </summary>
    public Request Body(string body)
    {
        _body = body;
        return this;
    }

    /// <summary>
    /// Executes the HTTP request and returns a response.
    /// </summary>
    public async Task<BaseResponse> FetchAsync()
    {
        var uri = _uriBuilder?.Build() ?? _baseUri;
        using var client = new HttpClient();
        using var request = new HttpRequestMessage(new HttpMethod(_method), uri);
        foreach (var header in _headers)
        {
            request.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }
        if (_body != null)
        {
            request.Content = new StringContent(_body);
        }
        var response = await client.SendAsync(request);
        return new BaseResponse(response);
    }

    /// <summary>
    /// Executes the HTTP request synchronously and returns a response.
    /// </summary>
    public BaseResponse Fetch()
    {
        return FetchAsync().GetAwaiter().GetResult();
    }
}
