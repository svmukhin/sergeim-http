// SPDX-FileCopyrightText: Copyright (c) [2025-2026] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using System.Net.Http.Headers;

namespace SergeiM.Http.Wire;

internal static class WireHelper
{
    internal static HttpRequestMessage BuildRequest(string method, string uri, Dictionary<string, string> headers, string? body)
    {
        var request = new HttpRequestMessage(new HttpMethod(method), uri);
        string? contentType = null;
        var headersCopy = new Dictionary<string, string>(headers);
        if (headersCopy.ContainsKey("Content-Type"))
        {
            contentType = headersCopy["Content-Type"];
            _ = headersCopy.Remove("Content-Type");
        }
        foreach (KeyValuePair<string, string> header in headersCopy)
        {
            _ = request.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }
        if (body != null)
        {
            request.Content = contentType != null
                ? new StringContent(body, System.Text.Encoding.UTF8, MediaTypeHeaderValue.Parse(contentType))
                : new StringContent(body);
        }
        return request;
    }
}
