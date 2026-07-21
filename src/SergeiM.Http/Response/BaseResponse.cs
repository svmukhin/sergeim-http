// SPDX-FileCopyrightText: Copyright (c) [2025-2026] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using SergeiM.Http.Request;

namespace SergeiM.Http.Response;

/// <summary>
/// Base HTTP response wrapper.
/// </summary>
public class BaseResponse : IResponse
{
    /// <summary>The underlying HTTP response message.</summary>
    protected readonly HttpResponseMessage _response;
    /// <summary>Cached response content string.</summary>
    protected string? _content;

    /// <summary>
    /// Initializes a new instance of the BaseResponse class.
    /// </summary>
    /// <param name="response">The HTTP response message to wrap.</param>
    public BaseResponse(HttpResponseMessage response)
    {
        _response = response;
    }

    /// <inheritdoc/>
    public int StatusCode => (int)_response.StatusCode;

    /// <inheritdoc/>
    public string Content
    {
        get
        {
            _content ??= _response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return _content;
        }
    }

    /// <inheritdoc/>
    public T As<T>() where T : IResponse
    {
        return (T)Activator.CreateInstance(typeof(T), _response)!;
    }

    /// <inheritdoc/>
    public virtual IResponse AssertStatus(int expectedStatus)
    {
        if (StatusCode != expectedStatus)
        {
            throw new HttpRequestException(
                $"Expected status {expectedStatus} but got {StatusCode}. Response: {Content}");
        }
        return this;
    }

    /// <inheritdoc/>
    public virtual string? GetHeader(string name)
    {
        if (_response.Headers.TryGetValues(name, out IEnumerable<string>? values))
        {
            return values.FirstOrDefault();
        }
        if (_response.Content.Headers.TryGetValues(name, out values))
        {
            return values.FirstOrDefault();
        }
        return null;
    }

    /// <inheritdoc/>
    public virtual IRequest Rel(string href)
    {
        // If href is already an absolute URL, use it directly
        if (Uri.IsWellFormedUriString(href, UriKind.Absolute))
        {
            return new BaseRequest(href);
        }

        // Otherwise, construct it relative to the current request
        string baseUri = _response.RequestMessage?.RequestUri?.GetLeftPart(UriPartial.Authority)
                      ?? throw new InvalidOperationException("Cannot determine base URI");

        string absoluteUri = new Uri(new Uri(baseUri), href).ToString();
        return new BaseRequest(absoluteUri);
    }

    /// <inheritdoc/>
    public virtual IRequest Header(string name, string value)
    {
        throw new InvalidOperationException(
            "Cannot add headers directly to a response. Use Rel() to create a new request first.");
    }
}
