// SPDX-FileCopyrightText: Copyright (c) [2025-2026] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

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
    /// <inheritdoc/>
    public async Task<HttpResponseMessage> SendAsync(string method, string uri, Dictionary<string, string> headers, string? body = null)
    {
        using var request = WireHelper.BuildRequest(method, uri, headers, body);
        return await _client.SendAsync(request);
    }

    /// <inheritdoc/>
    public HttpResponseMessage Send(string method, string uri, Dictionary<string, string> headers, string? body = null)
    {
        return SendAsync(method, uri, headers, body).GetAwaiter().GetResult();
    }
}
