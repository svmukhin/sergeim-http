// SPDX-FileCopyrightText: Copyright (c) [2025] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using System.Text.Json;
using SergeiM.Json;
using SergeiM.Json.IO;

namespace SergeiM.Http.Response;

/// <summary>
/// HTTP response with JSON parsing capabilities using SergeiM.Json library.
/// Provides direct access to immutable JSON types.
/// </summary>
public class JsonResponse : BaseResponse
{
    private JsonValue? _jsonValue;

    /// <summary>
    /// Initializes a new instance of the JsonResponse class.
    /// </summary>
    /// <param name="response">The HTTP response message to wrap.</param>
    public JsonResponse(HttpResponseMessage response) : base(response)
    {
    }

    /// <summary>
    /// Gets the parsed JSON value (immutable).
    /// </summary>
    public JsonValue Value
    {
        get
        {
            if (_jsonValue == null)
            {
                using var reader = JsonReader.Create(new StringReader(Content));
                _jsonValue = reader.Read();
            }
            return _jsonValue;
        }
    }

    /// <summary>
    /// Gets the JSON value as a JsonObject.
    /// </summary>
    /// <returns>The JSON object.</returns>
    /// <exception cref="InvalidOperationException">If the JSON value is not an object.</exception>
    public JsonObject AsObject()
    {
        return Value as JsonObject
            ?? throw new InvalidOperationException("JSON value is not an object.");
    }

    /// <summary>
    /// Gets the JSON value as a JsonArray.
    /// </summary>
    /// <returns>The JSON array.</returns>
    /// <exception cref="InvalidOperationException">If the JSON value is not an array.</exception>
    public JsonArray AsArray()
    {
        return Value as JsonArray
            ?? throw new InvalidOperationException("JSON value is not an array.");
    }

    /// <summary>
    /// Deserializes the JSON response directly to the specified type using System.Text.Json.
    /// </summary>
    /// <typeparam name="T">The type to deserialize to.</typeparam>
    /// <returns>The deserialized object.</returns>
    public T? Deserialize<T>()
    {
        return JsonSerializer.Deserialize<T>(Content);
    }

    /// <summary>
    /// Deserializes the JSON response directly to the specified type with custom options.
    /// </summary>
    /// <typeparam name="T">The type to deserialize to.</typeparam>
    /// <param name="options">Options to control the behavior during deserialization.</param>
    /// <returns>The deserialized object.</returns>
    public T? Deserialize<T>(JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize<T>(Content, options);
    }
}
