namespace FluentHttp.Wire;

/// <summary>
/// Wire decorator that adds authorization header to requests.
/// </summary>
public class BasicAuthWire : IWire
{
    private readonly string _value;
    private readonly IWire _origin;

    /// <summary>
    /// Creates a new authorization wire with Bearer token.
    /// </summary>
    public BasicAuthWire(IWire origin, string token)
    {
        _origin = origin;
        _value = token;
    }

    /// <summary>
    /// Sends an HTTP request asynchronously with authorization header.
    /// </summary>
    public Task<HttpResponseMessage> SendAsync(string method, string uri, Dictionary<string, string> headers, string? body = null)
    {
        return _origin.SendAsync(method, uri, new Dictionary<string, string>(headers)
        {
            [HttpHeaders.AUTHORIZATION] = _value
        }, body);
    }

    /// <summary>
    /// Sends an HTTP request synchronously with authorization header.
    /// </summary>
    public HttpResponseMessage Send(string method, string uri, Dictionary<string, string> headers, string? body = null)
    {
        return _origin.Send(method, uri, new Dictionary<string, string>(headers)
        {
            [HttpHeaders.AUTHORIZATION] = _value
        }, body);
    }
}
