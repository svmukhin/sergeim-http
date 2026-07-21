// SPDX-FileCopyrightText: Copyright (c) [2025-2026] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

namespace SergeiM.Http;

/// <summary>
/// Fluent URI builder for constructing request URIs with paths and query parameters.
/// </summary>
public interface IUriBuilder
{
    /// <summary>
    /// Returns to the parent request builder.
    /// </summary>
    /// <returns>The parent request instance.</returns>
    public IRequest Back();

    /// <summary>
    /// Builds and returns the complete URI as a string.
    /// </summary>
    /// <returns>The constructed URI.</returns>
    public string Build();

    /// <summary>
    /// Appends a path segment to the URI.
    /// </summary>
    /// <param name="pathSegment">The path segment to append (e.g., "/users" or "users").</param>
    /// <returns>The current URI builder instance for method chaining.</returns>
    public IUriBuilder Path(string pathSegment);

    /// <summary>
    /// Adds a query parameter to the URI.
    /// </summary>
    /// <param name="name">The query parameter name.</param>
    /// <param name="value">The query parameter value. Will be converted to string and URL-encoded.</param>
    /// <returns>The current URI builder instance for method chaining.</returns>
    public IUriBuilder QueryParam(string name, object value);
}
