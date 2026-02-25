// SPDX-FileCopyrightText: Copyright (c) [2025-2026] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using System.Net;
using SergeiM.Http.Tests.Wire.Mocks;

namespace SergeiM.Http.Tests.Wire;

/// <summary>
/// Base class for wire tests with common test scenarios.
/// </summary>
public abstract class WireTestBase
{
    protected abstract IWire CreateWire(HttpClient client);

    [TestMethod]
    public async Task SendAsync_ShouldSendBasicGetRequest()
    {
        var handler = new MockHttpMessageHandler((request) =>
        {
            Assert.AreEqual(HttpMethod.Get, request.Method);
            Assert.AreEqual("https://api.example.com/test", request.RequestUri?.ToString());
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Success")
            };
        });
        var response = await CreateWire(new HttpClient(handler)).SendAsync("GET", "https://api.example.com/test", []);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual("Success", content);
    }

    [TestMethod]
    public async Task SendAsync_ShouldIncludeHeaders()
    {
        var handler = new MockHttpMessageHandler((request) =>
        {
            Assert.IsTrue(request.Headers.Contains("X-Custom-Header"));
            Assert.AreEqual("CustomValue", request.Headers.GetValues("X-Custom-Header").First());
            return new HttpResponseMessage(HttpStatusCode.OK);
        });
        var response = await CreateWire(new HttpClient(handler)).SendAsync("GET", "https://api.example.com", new Dictionary<string, string>
        {
            ["X-Custom-Header"] = "CustomValue"
        });
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    public async Task SendAsync_ShouldIncludeBody()
    {
        var handler = new MockHttpMessageHandler(async (request) =>
        {
            var body = await request.Content!.ReadAsStringAsync();
            Assert.AreEqual("{\"test\":\"data\"}", body);
            return new HttpResponseMessage(HttpStatusCode.OK);
        });
        var response = await CreateWire(new HttpClient(handler)).SendAsync("POST", "https://api.example.com", [], "{\"test\":\"data\"}");
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    public void Send_ShouldWorkSynchronously()
    {
        var handler = new MockHttpMessageHandler((request) =>
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        });
        var response = CreateWire(new HttpClient(handler)).Send("GET", "https://api.example.com", []);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    public async Task SendAsync_ShouldSetContentType_WhenBareMediaType()
    {
        var handler = new MockHttpMessageHandler((request) =>
        {
            var ct = request.Content!.Headers.ContentType;
            Assert.AreEqual("application/json", ct!.MediaType);
            return new HttpResponseMessage(HttpStatusCode.OK);
        });
        await CreateWire(new HttpClient(handler)).SendAsync("POST", "https://api.example.com", new Dictionary<string, string>
        {
            ["Content-Type"] = "application/json"
        }, "{\"test\":\"data\"}");
    }

    [TestMethod]
    public async Task SendAsync_ShouldSetContentType_WhenContentTypeHasCharsetParameter()
    {
        var handler = new MockHttpMessageHandler((request) =>
        {
            var ct = request.Content!.Headers.ContentType;
            Assert.AreEqual("text/xml", ct!.MediaType);
            Assert.AreEqual("utf-8", ct.CharSet);
            return new HttpResponseMessage(HttpStatusCode.OK);
        });
        await CreateWire(new HttpClient(handler)).SendAsync("POST", "https://api.example.com", new Dictionary<string, string>
        {
            ["Content-Type"] = "text/xml; charset=utf-8"
        }, "<root/>");
    }

    [TestMethod]
    public async Task SendAsync_ShouldSetContentType_WhenContentTypeHasActionParameter()
    {
        const string soapAction = "http://example.com/MyService/MyAction";
        var handler = new MockHttpMessageHandler((request) =>
        {
            var ct = request.Content!.Headers.ContentType;
            Assert.AreEqual("application/soap+xml", ct!.MediaType);
            var actionParam = ct.Parameters.FirstOrDefault(p => p.Name == "action");
            Assert.IsNotNull(actionParam);
            Assert.AreEqual($"\"{soapAction}\"", actionParam.Value);
            return new HttpResponseMessage(HttpStatusCode.OK);
        });
        await CreateWire(new HttpClient(handler)).SendAsync("POST", "https://api.example.com", new Dictionary<string, string>
        {
            ["Content-Type"] = $"application/soap+xml; action=\"{soapAction}\""
        }, "<Envelope/>");
    }
}
