namespace FluentHttp;

/// <summary>
/// Wire decorator that adds retry logic to requests.
/// </summary>
public class RetryWire : IWire
{
    private readonly IWire _origin;
    private readonly int _maxRetries;
    private readonly TimeSpan _delayBetweenRetries;
    private readonly Func<HttpResponseMessage, bool>? _shouldRetry;

    /// <summary>
    /// Creates a new retry wire with default settings (3 retries, 1 second delay).
    /// </summary>
    public RetryWire(IWire origin)
        : this(origin, 3, TimeSpan.FromSeconds(1))
    {
    }

    /// <summary>
    /// Creates a new retry wire with custom retry count and delay.
    /// </summary>
    public RetryWire(IWire origin, int maxRetries, TimeSpan delayBetweenRetries)
    {
        _origin = origin;
        _maxRetries = maxRetries;
        _delayBetweenRetries = delayBetweenRetries;
        _shouldRetry = null;
    }

    /// <summary>
    /// Creates a new retry wire with custom retry logic.
    /// </summary>
    public RetryWire(IWire origin, int maxRetries, TimeSpan delayBetweenRetries, Func<HttpResponseMessage, bool> shouldRetry)
    {
        _origin = origin;
        _maxRetries = maxRetries;
        _delayBetweenRetries = delayBetweenRetries;
        _shouldRetry = shouldRetry;
    }

    /// <summary>
    /// Sends an HTTP request asynchronously with retry logic.
    /// </summary>
    public async Task<HttpResponseMessage> SendAsync(string method, string uri, Dictionary<string, string> headers, string? body = null)
    {
        Exception? lastException = null;
        for (int attempt = 0; attempt <= _maxRetries; attempt++)
        {
            try
            {
                var response = await _origin.SendAsync(method, uri, headers, body);
                if (attempt < _maxRetries && ShouldRetryResponse(response))
                {
                    await Task.Delay(_delayBetweenRetries);
                    continue;
                }
                return response;
            }
            catch (Exception ex) when (attempt < _maxRetries && IsTransientError(ex))
            {
                lastException = ex;
                await Task.Delay(_delayBetweenRetries);
            }
        }
        throw new InvalidOperationException($"Request failed after {_maxRetries + 1} attempts", lastException);
    }

    /// <summary>
    /// Sends an HTTP request synchronously with retry logic.
    /// </summary>
    public HttpResponseMessage Send(string method, string uri, Dictionary<string, string> headers, string? body = null)
    {
        return SendAsync(method, uri, headers, body).GetAwaiter().GetResult();
    }

    private bool ShouldRetryResponse(HttpResponseMessage response)
    {
        if (_shouldRetry != null)
        {
            return _shouldRetry(response);
        }
        return (int)response.StatusCode >= 500 || response.StatusCode == System.Net.HttpStatusCode.TooManyRequests;
    }

    private static bool IsTransientError(Exception ex)
    {
        return ex is HttpRequestException or TaskCanceledException or TimeoutException;
    }
}
