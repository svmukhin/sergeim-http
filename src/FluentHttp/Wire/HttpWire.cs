namespace FluentHttp.Wire;

/// <summary>
/// Basic HTTP wire implementation using HttpClient.
/// </summary>
public class HttpWire : IWire
{
    private readonly HttpClient _client;

    /// <summary>
    /// Creates a new HttpWire with a default HttpClient.
    /// </summary>
    public HttpWire()
    {
        _client = new HttpClient();
    }

    /// <summary>
    /// Creates a new HttpWire with a custom HttpClient.
    /// </summary>
    public HttpWire(HttpClient client)
    {
        _client = client;
    }

    /// <summary>
    /// Sends an HTTP request asynchronously.
    /// </summary>
    public async Task<HttpResponseMessage> SendAsync(string method, string uri, Dictionary<string, string> headers, string? body = null)
    {
        using var request = new HttpRequestMessage(new HttpMethod(method), uri);
        foreach (var header in headers)
        {
            request.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }
        if (body != null)
        {
            request.Content = new StringContent(body);
        }
        return await _client.SendAsync(request);
    }

    /// <summary>
    /// Sends an HTTP request synchronously.
    /// </summary>
    public HttpResponseMessage Send(string method, string uri, Dictionary<string, string> headers, string? body = null)
    {
        return SendAsync(method, uri, headers, body).GetAwaiter().GetResult();
    }
}
