using FluentHttp.Response;

namespace FluentHttp.Tests;

[TestClass]
public class JsonResponseTests
{
    [TestMethod]
    public void JsonResponse_ShouldParseJsonObject()
    {
        var json = """{"name": "John Doe", "age": 30, "active": true}""";
        var response = new JsonResponse(CreateMockJsonResponse(json));
        var name = response.Json().GetJsonObject().GetString("name");
        var age = response.Json().GetJsonObject().GetInt("age");
        var active = response.Json().GetJsonObject().GetBoolean("active");
        Assert.AreEqual("John Doe", name);
        Assert.AreEqual(30, age);
        Assert.AreEqual(true, active);
    }

    [TestMethod]
    public void JsonResponse_ShouldAccessNestedProperties()
    {
        var json = """{"user": {"name": "Jane", "email": "jane@example.com"}}""";
        var response = new JsonResponse(CreateMockJsonResponse(json));
        var name = response.Json().GetJsonObject()
            .GetJsonObject("user")
            .GetString("name");
        Assert.AreEqual("Jane", name);
    }

    [TestMethod]
    public void JsonArray_ShouldAccessElements()
    {
        var json = """{"items": ["apple", "banana", "cherry"]}""";
        var response = new JsonResponse(CreateMockJsonResponse(json));
        var items = response.Json().GetJsonObject().GetJsonArray("items");
        Assert.AreEqual(3, items.Length);
        Assert.AreEqual("apple", items.GetString(0));
        Assert.AreEqual("banana", items.GetString(1));
    }

    private static HttpResponseMessage CreateMockJsonResponse(string json)
    {
        var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };
        return response;
    }
}
