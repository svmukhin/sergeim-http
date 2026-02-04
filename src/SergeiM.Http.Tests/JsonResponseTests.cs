// SPDX-FileCopyrightText: Copyright (c) [2025-2026] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using SergeiM.Http.Response;

namespace SergeiM.Http.Tests;

[TestClass]
public class JsonResponseTests
{
    [TestMethod]
    public void JsonResponse_ShouldParseJsonObject()
    {
        var json = """{"name": "John Doe", "age": 30, "active": true}""";
        var response = new JsonResponse(CreateMockJsonResponse(json));
        var obj = response.AsObject();
        var name = obj.GetString("name");
        var age = obj.GetInt("age");
        var active = obj.GetBoolean("active");
        Assert.AreEqual("John Doe", name);
        Assert.AreEqual(30, age);
        Assert.AreEqual(true, active);
    }

    [TestMethod]
    public void JsonResponse_ShouldAccessNestedProperties()
    {
        var json = @"{""user"": {""name"": ""Jane"", ""email"": ""jane@example.com""}}";
        var response = new JsonResponse(CreateMockJsonResponse(json));
        var obj = response.AsObject();
        var name = obj.GetJsonObject("user")!.GetString("name");
        Assert.AreEqual("Jane", name);
    }

    [TestMethod]
    public void JsonArray_ShouldAccessElements()
    {
        var json = @"{""items"": [""apple"", ""banana"", ""cherry""]}";
        var response = new JsonResponse(CreateMockJsonResponse(json));
        var obj = response.AsObject();
        var items = obj.GetJsonArray("items")!;
        Assert.AreEqual(3, items.Count);
        Assert.AreEqual("apple", items.GetString(0));
        Assert.AreEqual("banana", items.GetString(1));
    }

    [TestMethod]
    public void JsonResponse_AssertStatus_ShouldReturnJsonResponse()
    {
        var json = @"{""result"": ""success""}";
        var response = new JsonResponse(CreateMockJsonResponse(json));
        var result = response.AssertStatus(200).AsObject();
        Assert.AreEqual("success", result.GetString("result"));
    }

    [TestMethod]
    public void JsonResponse_AssertStatus_AfterAs_ShouldAllowChaining()
    {
        var json = @"{""value"": 42}";
        var httpResponse = CreateMockJsonResponse(json);
        var baseResponse = new BaseResponse(httpResponse);
        var result = baseResponse.As<JsonResponse>().AssertStatus(200).AsObject();
        Assert.AreEqual(42, result.GetInt("value"));
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
