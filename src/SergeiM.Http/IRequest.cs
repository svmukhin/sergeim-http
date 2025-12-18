// SPDX-FileCopyrightText: Copyright (c) [2025] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using SergeiM.Http.Response;

namespace SergeiM.Http;

/// <summary>
/// Fluent HTTP request builder.
/// </summary>
public interface IRequest
{
    /// <summary>
    /// Sets the request body content.
    /// </summary>
    /// <param name="body">The body content to send with the request.</param>
    /// <returns>The current request instance for method chaining.</returns>
    IRequest Body(string body);

    /// <summary>
    /// Sets the request body content with a specific Content-Type.
    /// </summary>
    /// <param name="body">The body content to send with the request.</param>
    /// <param name="contentType">The Content-Type header value (e.g., "application/json").</param>
    /// <returns>The current request instance for method chaining.</returns>
    IRequest Body(string body, string contentType);

    /// <summary>
    /// Sets the request body as JSON by serializing the provided object.
    /// Automatically sets Content-Type to application/json.
    /// </summary>
    /// <param name="obj">The object to serialize as JSON.</param>
    /// <returns>The current request instance for method chaining.</returns>
    IRequest JsonBody(object obj);

    /// <summary>
    /// Executes the HTTP request synchronously and returns the response.
    /// </summary>
    /// <returns>The HTTP response.</returns>
    BaseResponse Fetch();

    /// <summary>
    /// Executes the HTTP request asynchronously and returns the response.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the HTTP response.</returns>
    Task<BaseResponse> FetchAsync();

    /// <summary>
    /// Adds or updates an HTTP header for the request.
    /// </summary>
    /// <param name="name">The header name.</param>
    /// <param name="value">The header value.</param>
    /// <returns>The current request instance for method chaining.</returns>
    IRequest Header(string name, string value);

    /// <summary>
    /// Sets the HTTP method for the request (GET, POST, PUT, DELETE, etc.).
    /// </summary>
    /// <param name="method">The HTTP method to use.</param>
    /// <returns>The current request instance for method chaining.</returns>
    IRequest Method(string method);

    /// <summary>
    /// Changes the wire implementation used to send the request.
    /// Allows wrapping with decorators for additional functionality like authorization, retries, etc.
    /// </summary>
    /// <param name="wire">The wire implementation to use for sending the request.</param>
    /// <returns>The current request instance for method chaining.</returns>
    IRequest Through(IWire wire);

    /// <summary>
    /// Gets the URI builder for constructing the request URI with path segments and query parameters.
    /// </summary>
    /// <returns>A URI builder instance for fluent URI construction.</returns>
    UriBuilder Uri();
}
