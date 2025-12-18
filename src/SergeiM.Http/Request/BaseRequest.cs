// SPDX-FileCopyrightText: Copyright (c) [2025] [Sergei Mukhin]
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

    private readonly string _baseUri;
    private UriBuilder? _uriBuilder;
    private string _method = GET;
    private readonly Dictionary<string, string> _headers;
    private string? _body;
    private string? _contentType;
    private IWire _wire;

    /// <summary>
    /// Creates a new HTTP request with the specified base URI.
    /// </summary>
    public BaseRequest(string uri) : this(uri, new HttpWire())
    {
    }

    /// <summary>
    /// Creates a new HTTP request with the specified base URI and wire.
    /// </summary>
    public BaseRequest(string uri, IWire wire)
    {
        _baseUri = uri;
        _headers = [];
        _wire = wire;
    }

    /// <inheritdoc/>
    public UriBuilder Uri()
    {
        _uriBuilder ??= new UriBuilder(this, _baseUri);
        return _uriBuilder;
    }

    /// <inheritdoc/>
    public IRequest Method(string method)
    {
        _method = method;
        return this;
    }

    /// <inheritdoc/>
    public IRequest Header(string name, string value)
    {
        _headers[name] = value;
        return this;
    }

    /// <inheritdoc/>
    public IRequest Body(string body)
    {
        _body = body;
        return this;
    }

    /// <inheritdoc/>
    public IRequest Body(string body, string contentType)
    {
        _body = body;
        _contentType = contentType;
        return this;
    }

    /// <inheritdoc/>
    public IRequest JsonBody(object obj)
    {
        _body = System.Text.Json.JsonSerializer.Serialize(obj);
        _contentType = MediaType.APPLICATION_JSON;
        return this;
    }

    /// <inheritdoc/>
    public IRequest Through(IWire wire)
    {
        _wire = wire;
        return this;
    }

    /// <inheritdoc/>
    public async Task<BaseResponse> FetchAsync()
    {
        var uri = _uriBuilder?.Build() ?? _baseUri;
        var headers = new Dictionary<string, string>(_headers);
        
        // Add Content-Type header if body and contentType are set
        if (_body != null && _contentType != null && !headers.ContainsKey(HttpHeaders.CONTENT_TYPE))
        {
            headers[HttpHeaders.CONTENT_TYPE] = _contentType;
        }
        
        var response = await _wire.SendAsync(_method, uri, headers, _body);
        return new BaseResponse(response);
    }

    /// <inheritdoc/>
    public BaseResponse Fetch()
    {
        return FetchAsync().GetAwaiter().GetResult();
    }
}
