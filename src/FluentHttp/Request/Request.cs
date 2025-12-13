using FluentHttp.Response;
using FluentHttp.Wire;

namespace FluentHttp.Request;

/// <summary>
/// Fluent HTTP request builder.
/// </summary>
public class BaseRequest : IRequest
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
    private IWire _wire;

    /// <summary>
    /// Creates a new HTTP request with the specified base URI.
    /// </summary>
    public BaseRequest(string uri) : this(uri, new HttpWire())
    {
    }

    /// <summary>
    /// Creates a new HTTP request with the specified base URI and wire.
    /// </summary>
    public BaseRequest(string uri, IWire wire)
    {
        _baseUri = uri;
        _headers = [];
        _wire = wire;
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
    public IRequest Method(string method)
    {
        _method = method;
        return this;
    }

    /// <summary>
    /// Adds a header to the request.
    /// </summary>
    public IRequest Header(string name, string value)
    {
        _headers[name] = value;
        return this;
    }

    /// <summary>
    /// Sets the request body.
    /// </summary>
    public IRequest Body(string body)
    {
        _body = body;
        return this;
    }

    /// <summary>
    /// Changes the wire used to send the request.
    /// </summary>
    public IRequest Through(IWire wire)
    {
        _wire = wire;
        return this;
    }

    /// <summary>
    /// Executes the HTTP request and returns a response.
    /// </summary>
    public async Task<BaseResponse> FetchAsync()
    {
        var uri = _uriBuilder?.Build() ?? _baseUri;
        var response = await _wire.SendAsync(_method, uri, _headers, _body);
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
