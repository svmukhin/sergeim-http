namespace FluentHttp;

/// <summary>
/// Represents a wire that can send HTTP requests.
/// Implementations can be wrapped to add functionalities like authorization, retries, etc.
/// </summary>
public interface IWire
{
    /// <summary>
    /// Sends an HTTP request asynchronously.
    /// </summary>
    /// <param name="method">The HTTP method (GET, POST, etc.)</param>
    /// <param name="uri">The request URI</param>
    /// <param name="headers">The request headers</param>
    /// <param name="body">The request body (optional)</param>
    /// <returns>The HTTP response</returns>
    Task<HttpResponseMessage> SendAsync(string method, string uri, Dictionary<string, string> headers, string? body = null);

    /// <summary>
    /// Sends an HTTP request synchronously.
    /// </summary>
    /// <param name="method">The HTTP method (GET, POST, etc.)</param>
    /// <param name="uri">The request URI</param>
    /// <param name="headers">The request headers</param>
    /// <param name="body">The request body (optional)</param>
    /// <returns>The HTTP response</returns>
    HttpResponseMessage Send(string method, string uri, Dictionary<string, string> headers, string? body = null);
}
