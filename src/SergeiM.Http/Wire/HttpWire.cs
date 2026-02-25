// SPDX-FileCopyrightText: Copyright (c) [2025-2026] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using System.Net.Http.Headers;

namespace SergeiM.Http.Wire;

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

    /// <inheritdoc/>
    public async Task<HttpResponseMessage> SendAsync(string method, string uri, Dictionary<string, string> headers, string? body = null)
    {
        using var request = new HttpRequestMessage(new HttpMethod(method), uri);
        string? contentType = null;
        var headersCopy = new Dictionary<string, string>(headers);
        if (headersCopy.ContainsKey("Content-Type"))
        {
            contentType = headersCopy["Content-Type"];
            headersCopy.Remove("Content-Type");
        }
        foreach (var header in headersCopy)
        {
            request.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }
        if (body != null)
        {
            request.Content = contentType != null
                ? new StringContent(body, System.Text.Encoding.UTF8, MediaTypeHeaderValue.Parse(contentType))
                : new StringContent(body);
        }
        return await _client.SendAsync(request);
    }

    /// <inheritdoc/>
    public HttpResponseMessage Send(string method, string uri, Dictionary<string, string> headers, string? body = null)
    {
        return SendAsync(method, uri, headers, body).GetAwaiter().GetResult();
    }
}
