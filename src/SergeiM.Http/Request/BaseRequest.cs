// SPDX-FileCopyrightText: Copyright (c) [2025-2026] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using SergeiM.Http.Response;
using SergeiM.Http.Wire;

namespace SergeiM.Http.Request;

/// <summary>
/// Fluent HTTP request builder.
/// </summary>
public class BaseRequest : IRequest
{
    /// <summary>HTTP GET method constant.</summary>
    public const string GET = "GET";
    /// <summary>HTTP POST method constant.</summary>
    public const string POST = "POST";
    /// <summary>HTTP PUT method constant.</summary>
    public const string PUT = "PUT";
    /// <summary>HTTP DELETE method constant.</summary>
    public const string DELETE = "DELETE";
    /// <summary>HTTP PATCH method constant.</summary>
    public const string PATCH = "PATCH";
    /// <summary>HTTP HEAD method constant.</summary>
    public const string HEAD = "HEAD";
    /// <summary>HTTP OPTIONS method constant.</summary>
    public const string OPTIONS = "OPTIONS";

    private readonly string _home;
    private readonly string _method;
    private readonly Dictionary<string, string> _headers;
    private readonly string? _body;
    private readonly string? _contentType;
    private readonly IWire _wire;

    /// <summary>
    /// Creates a new HTTP request with the specified base URI.
    /// </summary>
    public BaseRequest(string uri)
        : this(uri, new HttpWire())
    {
    }

    /// <summary>
    /// Creates a new HTTP request with the specified base URI and wire implementation.
    /// </summary>
    /// <param name="uri">The base URI.</param>
    /// <param name="wire">The wire implementation.</param>
    public BaseRequest(string uri, IWire wire)
        : this(uri, wire, GET, [], null, null)
    {
    }

    /// <summary>
    /// Creates a new HTTP request with the specified base URI, wire, and HTTP method.
    /// </summary>
    /// <param name="uri">The base URI.</param>
    /// <param name="wire">The wire implementation.</param>
    /// <param name="method">The HTTP method.</param>
    public BaseRequest(string uri, IWire wire, string method)
        : this(uri, wire, method, [], null, null)
    {
    }

    /// <summary>
    /// Creates a new HTTP request with the specified base URI, wire, HTTP method, and headers.
    /// </summary>
    /// <param name="uri">The base URI.</param>
    /// <param name="wire">The wire implementation.</param>
    /// <param name="method">The HTTP method.</param>
    /// <param name="headers">The HTTP headers.</param>
    public BaseRequest(string uri, IWire wire, string method, Dictionary<string, string> headers)
        : this(uri, wire, method, headers, null, null)
    {
    }

    /// <summary>
    /// Creates a new HTTP request with all parameters specified.
    /// </summary>
    /// <param name="home">The base URI.</param>
    /// <param name="wire">The wire implementation.</param>
    /// <param name="method">The HTTP method.</param>
    /// <param name="headers">The HTTP headers.</param>
    /// <param name="body">The request body.</param>
    /// <param name="contentType">The content type.</param>
    public BaseRequest(
        string home,
        IWire wire,
        string method,
        Dictionary<string, string> headers,
        string? body,
        string? contentType)
    {
        _home = home;
        _method = method;
        _headers = headers;
        _body = body;
        _contentType = contentType;
        _wire = wire;
    }

    /// <inheritdoc/>
    public IUriBuilder Uri()
    {
        return new UriBuilder(this, _home);
    }

    /// <inheritdoc/>
    public IRequest Uri(string uri)
    {
        return new BaseRequest(uri, _wire, _method, _headers, _body, _contentType);
    }

    /// <inheritdoc/>
    public IRequest Method(string method)
    {
        return new BaseRequest(_home, _wire, method, _headers, _body, _contentType);
    }

    /// <inheritdoc/>
    public IRequest Header(string name, string value)
    {
        return new BaseRequest(_home, _wire, _method, new Dictionary<string, string>(_headers)
        {
            [name] = value
        }, _body, _contentType);
    }

    /// <inheritdoc/>
    public IRequest Body(string body)
    {
        return new BaseRequest(_home, _wire, _method, _headers, body, _contentType);
    }

    /// <inheritdoc/>
    public IRequest Body(string body, string contentType)
    {
        return new BaseRequest(_home, _wire, _method, _headers, body, contentType);
    }

    /// <inheritdoc/>
    public IRequest JsonBody(object obj)
    {
        return new BaseRequest(_home, _wire, _method, _headers, System.Text.Json.JsonSerializer.Serialize(obj), MediaType.APPLICATION_JSON);
    }

    /// <inheritdoc/>
    public IRequest Through(IWire wire)
    {
        return new BaseRequest(_home, wire, _method, _headers, _body, _contentType);
    }

    /// <inheritdoc/>
    public async Task<BaseResponse> FetchAsync()
    {
        var headers = new Dictionary<string, string>(_headers);
        if (_body != null && _contentType != null && !headers.ContainsKey(HttpHeaders.CONTENT_TYPE))
        {
            headers[HttpHeaders.CONTENT_TYPE] = _contentType;
        }
        var response = await _wire.SendAsync(_method, _home, headers, _body);
        return new BaseResponse(response);
    }

    /// <inheritdoc/>
    public BaseResponse Fetch()
    {
        return FetchAsync().GetAwaiter().GetResult();
    }
}
