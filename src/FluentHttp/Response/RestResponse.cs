namespace FluentHttp;

public interface IResponse
{
    int StatusCode { get; }
    string Content { get; }

    T As<T>() where T : BaseResponse;
    BaseResponse AssertStatus(int expectedStatus);
    string? GetHeader(string name);
    IRequest Header(string name, string value);
    IRequest Rel(string href);
}

/// <summary>
/// Base HTTP response wrapper.
/// </summary>
public class BaseResponse : IResponse
{
    protected readonly HttpResponseMessage _response;
    protected string? _content;

    public BaseResponse(HttpResponseMessage response)
    {
        _response = response;
    }

    /// <summary>
    /// Gets the HTTP status code.
    /// </summary>
    public int StatusCode => (int)_response.StatusCode;

    /// <summary>
    /// Gets the response content as a string.
    /// </summary>
    public string Content
    {
        get
        {
            _content ??= _response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return _content;
        }
    }

    /// <summary>
    /// Converts this response to a specific response type.
    /// </summary>
    public T As<T>() where T : BaseResponse
    {
        return (T)Activator.CreateInstance(typeof(T), _response)!;
    }

    /// <summary>
    /// Asserts that the response has the expected status code.
    /// </summary>
    public BaseResponse AssertStatus(int expectedStatus)
    {
        if (StatusCode != expectedStatus)
        {
            throw new HttpRequestException(
                $"Expected status {expectedStatus} but got {StatusCode}. Response: {Content}");
        }
        return this;
    }

    /// <summary>
    /// Gets a header value from the response.
    /// </summary>
    public string? GetHeader(string name)
    {
        if (_response.Headers.TryGetValues(name, out var values))
        {
            return values.FirstOrDefault();
        }
        if (_response.Content.Headers.TryGetValues(name, out values))
        {
            return values.FirstOrDefault();
        }
        return null;
    }

    /// <summary>
    /// Creates a new request following a link relation.
    /// </summary>
    public IRequest Rel(string href)
    {
        // If href is already an absolute URL, use it directly
        if (Uri.IsWellFormedUriString(href, UriKind.Absolute))
        {
            return new Request(href);
        }

        // Otherwise, construct it relative to the current request
        var baseUri = _response.RequestMessage?.RequestUri?.GetLeftPart(UriPartial.Authority)
                      ?? throw new InvalidOperationException("Cannot determine base URI");

        var absoluteUri = new Uri(new Uri(baseUri), href).ToString();
        return new Request(absoluteUri);
    }

    /// <summary>
    /// Adds a header to a new request (for chaining).
    /// </summary>
    public IRequest Header(string name, string value)
    {
        throw new InvalidOperationException(
            "Cannot add headers directly to a response. Use Rel() to create a new request first.");
    }
}
