using System.Text.Json;

namespace FluentHttp.Response;

/// <summary>
/// HTTP response with JSON parsing capabilities.
/// </summary>
public class JsonResponse : BaseResponse
{
    private JsonDocument? _jsonDocument;
    private JsonElement? _rootElement;

    public JsonResponse(HttpResponseMessage response) : base(response)
    {
    }

    /// <summary>
    /// Gets the parsed JSON document.
    /// </summary>
    public JsonDocument JsonDocument
    {
        get
        {
            _jsonDocument ??= JsonDocument.Parse(Content);
            return _jsonDocument;
        }
    }

    /// <summary>
    /// Provides access to the JSON structure.
    /// </summary>
    public JsonNavigator Json()
    {
        return new JsonNavigator(this);
    }

    /// <summary>
    /// Gets the root JSON element.
    /// </summary>
    internal JsonElement RootElement
    {
        get
        {
            _rootElement ??= JsonDocument.RootElement;
            return _rootElement.Value;
        }
    }
}

/// <summary>
/// Navigator for traversing JSON structures.
/// </summary>
public class JsonNavigator
{
    private readonly JsonResponse _response;

    internal JsonNavigator(JsonResponse response)
    {
        _response = response;
    }

    /// <summary>
    /// Gets the root JSON element as a JsonObject.
    /// </summary>
    public JsonObject GetJsonObject()
    {
        return new JsonObject(_response.RootElement);
    }

    /// <summary>
    /// Gets the root JSON element as a JsonArray.
    /// </summary>
    public JsonArray GetJsonArray()
    {
        return new JsonArray(_response.RootElement);
    }
}

/// <summary>
/// Wrapper for JSON object operations.
/// </summary>
public class JsonObject
{
    private readonly JsonElement _element;

    internal JsonObject(JsonElement element)
    {
        if (element.ValueKind != JsonValueKind.Object)
        {
            throw new InvalidOperationException("JSON element is not an object.");
        }
        _element = element;
    }

    /// <summary>
    /// Gets a string property value.
    /// </summary>
    public string GetString(string propertyName)
    {
        if (_element.TryGetProperty(propertyName, out var property))
        {
            return property.GetString() ?? string.Empty;
        }
        throw new InvalidOperationException($"Property '{propertyName}' not found in JSON object.");
    }

    /// <summary>
    /// Gets an integer property value.
    /// </summary>
    public int GetInt(string propertyName)
    {
        if (_element.TryGetProperty(propertyName, out var property))
        {
            return property.GetInt32();
        }
        throw new InvalidOperationException($"Property '{propertyName}' not found in JSON object.");
    }

    /// <summary>
    /// Gets a boolean property value.
    /// </summary>
    public bool GetBoolean(string propertyName)
    {
        if (_element.TryGetProperty(propertyName, out var property))
        {
            return property.GetBoolean();
        }
        throw new InvalidOperationException($"Property '{propertyName}' not found in JSON object.");
    }

    /// <summary>
    /// Gets a nested JSON object.
    /// </summary>
    public JsonObject GetJsonObject(string propertyName)
    {
        if (_element.TryGetProperty(propertyName, out var property))
        {
            return new JsonObject(property);
        }
        throw new InvalidOperationException($"Property '{propertyName}' not found in JSON object.");
    }

    /// <summary>
    /// Gets a JSON array property.
    /// </summary>
    public JsonArray GetJsonArray(string propertyName)
    {
        if (_element.TryGetProperty(propertyName, out var property))
        {
            return new JsonArray(property);
        }
        throw new InvalidOperationException($"Property '{propertyName}' not found in JSON object.");
    }
}

/// <summary>
/// Wrapper for JSON array operations.
/// </summary>
public class JsonArray
{
    private readonly JsonElement _element;

    internal JsonArray(JsonElement element)
    {
        if (element.ValueKind != JsonValueKind.Array)
        {
            throw new InvalidOperationException("JSON element is not an array.");
        }
        _element = element;
    }

    /// <summary>
    /// Gets the length of the array.
    /// </summary>
    public int Length => _element.GetArrayLength();

    /// <summary>
    /// Gets a JSON object at the specified index.
    /// </summary>
    public JsonObject GetJsonObject(int index)
    {
        var element = _element[index];
        return new JsonObject(element);
    }

    /// <summary>
    /// Gets a string value at the specified index.
    /// </summary>
    public string GetString(int index)
    {
        return _element[index].GetString() ?? string.Empty;
    }
}
