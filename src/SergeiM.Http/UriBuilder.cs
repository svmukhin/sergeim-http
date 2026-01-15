// SPDX-FileCopyrightText: Copyright (c) [2025-2026] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using System.Text;

namespace SergeiM.Http;

/// <summary>
/// Fluent URI builder for constructing request URIs.
/// </summary>
public class UriBuilder : IUriBuilder
{
    private readonly IRequest _request;
    private readonly StringBuilder _path;
    private readonly Dictionary<string, string> _queryParams;

    internal UriBuilder(IRequest request, string baseUri)
    {
        _request = request;
        _path = new StringBuilder();
        _queryParams = new Dictionary<string, string>();
        var uri = new Uri(baseUri);
        BaseUri = $"{uri.Scheme}://{uri.Authority}";
        if (!string.IsNullOrEmpty(uri.AbsolutePath) && uri.AbsolutePath != "/")
        {
            _path.Append(uri.AbsolutePath);
        }
        if (!string.IsNullOrEmpty(uri.Query))
        {
            ParseQueryString(uri.Query);
        }
    }

    internal string BaseUri { get; }

    /// <inheritdoc/>
    public IUriBuilder Path(string pathSegment)
    {
        if (!pathSegment.StartsWith("/"))
        {
            _path.Append('/');
        }
        _path.Append(pathSegment);
        return this;
    }

    /// <inheritdoc/>
    public IUriBuilder QueryParam(string name, object value)
    {
        _queryParams[name] = value.ToString() ?? string.Empty;
        return this;
    }

    /// <inheritdoc/>
    public IRequest Back()
    {
        return _request.Uri(Build());
    }

    /// <inheritdoc/>
    public string Build()
    {
        var sb = new StringBuilder(BaseUri);
        if (_path.Length > 0)
        {
            sb.Append(_path);
        }
        if (_queryParams.Count > 0)
        {
            sb.Append('?');
            sb.Append(string.Join("&", _queryParams.Select(kvp =>
                $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}")));
        }
        return sb.ToString();
    }

    private void ParseQueryString(string query)
    {
        if (query.StartsWith("?"))
        {
            query = query.Substring(1);
        }
        var pairs = query.Split('&');
        foreach (var pair in pairs)
        {
            var parts = pair.Split('=', 2);
            var key = Uri.UnescapeDataString(parts[0]);
            var value = parts.Length > 1 ? Uri.UnescapeDataString(parts[1]) : string.Empty;
            _queryParams[key] = value;
        }
    }
}
