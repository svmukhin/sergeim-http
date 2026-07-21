// SPDX-FileCopyrightText: Copyright (c) [2025-2026] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

namespace SergeiM.Http.Wire;

/// <summary>
/// Wire implementation that creates a fresh <see cref="HttpClient"/> from a factory delegate on every request.
/// Designed for use with <c>IHttpClientFactory</c> in long-running applications.
/// </summary>
public class ManagedHttpWire : IWire
{
    private readonly Func<HttpClient> _createClient;

    /// <summary>
    /// Creates a new ManagedHttpWire with a factory delegate that produces <see cref="HttpClient"/> instances.
    /// </summary>
    /// <param name="createClient">A delegate that returns a new <see cref="HttpClient"/>. Intended to wrap <c>IHttpClientFactory.CreateClient()</c>.</param>
    public ManagedHttpWire(Func<HttpClient> createClient)
    {
        _createClient = createClient;
    }

    /// <inheritdoc/>
    public async Task<HttpResponseMessage> SendAsync(string method, string uri, Dictionary<string, string> headers, string? body = null)
    {
        HttpClient client = _createClient();
        try
        {
            using HttpRequestMessage request = WireHelper.BuildRequest(method, uri, headers, body);
            return await client.SendAsync(request).ConfigureAwait(false);
        }
        finally
        {
            client.Dispose();
        }
    }

    /// <inheritdoc/>
    public HttpResponseMessage Send(string method, string uri, Dictionary<string, string> headers, string? body = null)
    {
        return SendAsync(method, uri, headers, body).GetAwaiter().GetResult();
    }
}
