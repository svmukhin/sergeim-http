// SPDX-FileCopyrightText: Copyright (c) [2025-2026] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using System.Net;
using SergeiM.Http.Tests.Wire.Mocks;
using SergeiM.Http.Wire;

namespace SergeiM.Http.Tests.Wire;

[TestClass]
public class RetryWireTests
{
    [TestMethod]
    public async Task SendAsync_ShouldReturnOnFirstSuccess()
    {
        int attemptCount = 0;
        var mockWire = new MockWire((method, uri, headers, body) =>
        {
            attemptCount++;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        });
        HttpResponseMessage response = await new RetryWire(
            mockWire,
            3,
            TimeSpan.FromMilliseconds(10)).SendAsync("GET", "https://api.example.com", []);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(1, attemptCount);
    }

    [TestMethod]
    public async Task SendAsync_ShouldRetryOnServerError()
    {
        int attemptCount = 0;
        var mockWire = new MockWire((method, uri, headers, body) =>
        {
            attemptCount++;
            if (attemptCount < 3)
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError));
            }
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        });
        HttpResponseMessage response = await new RetryWire(
            mockWire,
            3,
            TimeSpan.FromMilliseconds(10)).SendAsync("GET", "https://api.example.com", []);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(3, attemptCount);
    }

    [TestMethod]
    public async Task SendAsync_ShouldRetryOnTooManyRequests()
    {
        int attemptCount = 0;
        var mockWire = new MockWire((method, uri, headers, body) =>
        {
            attemptCount++;
            if (attemptCount < 2)
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.TooManyRequests));
            }
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        });
        HttpResponseMessage response = await new RetryWire(
            mockWire,
            3,
            TimeSpan.FromMilliseconds(10)).SendAsync("GET", "https://api.example.com", []);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(2, attemptCount);
    }

    [TestMethod]
    public async Task SendAsync_ShouldNotRetryOnClientError()
    {
        int attemptCount = 0;
        var mockWire = new MockWire((method, uri, headers, body) =>
        {
            attemptCount++;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest));
        });
        HttpResponseMessage response = await new RetryWire(
            mockWire,
            3,
            TimeSpan.FromMilliseconds(10)).SendAsync("GET", "https://api.example.com", []);
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.AreEqual(1, attemptCount);
    }

    [TestMethod]
    public async Task SendAsync_ShouldRespectMaxRetries()
    {
        int attemptCount = 0;
        var mockWire = new MockWire((method, uri, headers, body) =>
        {
            attemptCount++;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError));
        });
        HttpResponseMessage response = await new RetryWire(
            mockWire,
            2,
            TimeSpan.FromMilliseconds(10)).SendAsync("GET", "https://api.example.com", []);
        Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.AreEqual(3, attemptCount); // Initial + 2 retries
    }

    [TestMethod]
    public async Task SendAsync_ShouldRetryOnHttpRequestException()
    {
        int attemptCount = 0;
        var mockWire = new MockWire((method, uri, headers, body) =>
        {
            attemptCount++;
            if (attemptCount < 3)
            {
                throw new HttpRequestException("Network error");
            }
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        });
        HttpResponseMessage response = await new RetryWire(
            mockWire,
            3,
            TimeSpan.FromMilliseconds(10)).SendAsync("GET", "https://api.example.com", []);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(3, attemptCount);
    }

    [TestMethod]
    public async Task SendAsync_ShouldThrowAfterMaxRetriesOnException()
    {
        int attemptCount = 0;
        var mockWire = new MockWire((method, uri, headers, body) =>
        {
            attemptCount++;
            throw new HttpRequestException("Network error");
        });
        _ = await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () =>
        {
            _ = await new RetryWire(
                mockWire,
                2,
                TimeSpan.FromMilliseconds(10)).SendAsync("GET", "https://api.example.com", []);
        });
        Assert.AreEqual(3, attemptCount);
    }

    [TestMethod]
    public async Task SendAsync_ShouldUseCustomRetryLogic()
    {
        int attemptCount = 0;
        var mockWire = new MockWire((method, uri, headers, body) =>
        {
            attemptCount++;
            if (attemptCount < 2)
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
            }
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        });
        HttpResponseMessage response = await new RetryWire(
            mockWire,
            3,
            TimeSpan.FromMilliseconds(10),
            (response) => response.StatusCode == HttpStatusCode.NotFound).SendAsync("GET", "https://api.example.com", []);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(2, attemptCount);
    }

    [TestMethod]
    public void Constructor_ShouldUseDefaultSettings()
    {
        var mockWire = new MockWire((method, uri, headers, body) =>
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        });
        var retryWire = new RetryWire(mockWire);
        Assert.IsNotNull(retryWire);
    }

    [TestMethod]
    public async Task SendAsync_ShouldRetryOnTaskCanceledException()
    {
        int attemptCount = 0;
        var mockWire = new MockWire((method, uri, headers, body) =>
        {
            attemptCount++;
            if (attemptCount < 2)
            {
                throw new TaskCanceledException("Request timeout");
            }
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        });
        HttpResponseMessage response = await new RetryWire(
            mockWire,
            3,
            TimeSpan.FromMilliseconds(10)).SendAsync("GET", "https://api.example.com", []);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(2, attemptCount);
    }

    [TestMethod]
    public async Task SendAsync_ShouldRetryOnTimeoutException()
    {
        int attemptCount = 0;
        var mockWire = new MockWire((method, uri, headers, body) =>
        {
            attemptCount++;
            if (attemptCount < 2)
            {
                throw new TimeoutException("Request timeout");
            }
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        });
        HttpResponseMessage response = await new RetryWire(
            mockWire,
            3,
            TimeSpan.FromMilliseconds(10)).SendAsync("GET", "https://api.example.com", []);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(2, attemptCount);
    }
}
