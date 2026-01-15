// SPDX-FileCopyrightText: Copyright (c) [2025-2026] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using SergeiM.Http.Request;

namespace SergeiM.Http.Tests;

[TestClass]
public class RequestTests
{
    [TestMethod]
    public void Request_ShouldBuildSimpleUri()
    {
        var uri = new BaseRequest("https://www.example.com:8080").Uri().Path("/users").QueryParam("id", 333).Build();
        Assert.AreEqual("https://www.example.com:8080/users?id=333", uri);
    }

    [TestMethod]
    public void Request_ShouldSetMethod()
    {
        var request = new BaseRequest("https://www.example.com")
            .Method(BaseRequest.POST);
        Assert.IsNotNull(request);
    }

    [TestMethod]
    public void Request_ShouldSetHeaders()
    {
        var request = new BaseRequest("https://www.example.com")
            .Header(HttpHeaders.ACCEPT, MediaType.APPLICATION_JSON)
            .Header(HttpHeaders.USER_AGENT, "FluentHttp/1.0");
        Assert.IsNotNull(request);
    }

    [TestMethod]
    public void UriBuilder_ShouldHandleMultipleQueryParams()
    {
        var uri = new BaseRequest("https://api.example.com").Uri()
            .Path("/search")
            .QueryParam("q", "test")
            .QueryParam("page", 1)
            .QueryParam("size", 10)
            .Build();
        Assert.IsTrue(uri.Contains("q=test"));
        Assert.IsTrue(uri.Contains("page=1"));
        Assert.IsTrue(uri.Contains("size=10"));
    }

    [TestMethod]
    public void UriBuilder_ShouldReturnNewRequest()
    {
        var request = new BaseRequest("https://www.example.com");
        var back = request.Uri().Path("/test").Back();
        Assert.AreNotSame(request, back);
    }

    // @todo This test would require mocking HttpClient
    //  Left as a placeholder for integration testing
    [TestMethod]
    public void RestResponse_ShouldConvertToTypedResponse()
    {
        Assert.IsTrue(true);
    }
}
