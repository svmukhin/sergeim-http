// SPDX-FileCopyrightText: Copyright (c) [2025-2026] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using System.Net;
using SergeiM.Http.Tests.Wire.Mocks;
using SergeiM.Http.Wire;

namespace SergeiM.Http.Tests.Wire;

[TestClass]
public class WireDecoratorChainTests
{
    [TestMethod]
    public async Task ShouldChainMultipleDecorators()
    {
        var handler = new MockHttpMessageHandler((request) =>
        {
            Assert.IsTrue(request.Headers.Contains(HttpHeaders.AUTHORIZATION));
            return new HttpResponseMessage(HttpStatusCode.OK);
        });
        var wire = new BasicAuthWire(
            new RetryWire(
                new HttpWire(new HttpClient(handler)),
                2,
                TimeSpan.FromMilliseconds(10)
            ),
            "Bearer test-token"
        );
        HttpResponseMessage response = await wire.SendAsync("GET", "https://api.example.com", []);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    public async Task ShouldRetryWithAuthorization()
    {
        int attemptCount = 0;
        var mockWire = new MockWire((method, uri, headers, body) =>
        {
            attemptCount++;
            Assert.IsTrue(headers.ContainsKey(HttpHeaders.AUTHORIZATION));
            if (attemptCount < 2)
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError));
            }
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        });
        var wire = new RetryWire(
            new BasicAuthWire(mockWire, "Bearer test-token"),
            3,
            TimeSpan.FromMilliseconds(10)
        );
        HttpResponseMessage response = await wire.SendAsync("GET", "https://api.example.com", []);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(2, attemptCount);
    }

    [TestMethod]
    public async Task ShouldChainInReverseOrder()
    {
        int attemptCount = 0;
        var mockWire = new MockWire((method, uri, headers, body) =>
        {
            attemptCount++;
            Assert.IsTrue(headers.ContainsKey(HttpHeaders.AUTHORIZATION));
            if (attemptCount < 2)
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError));
            }
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        });
        var wire = new BasicAuthWire(
            new RetryWire(mockWire, 3, TimeSpan.FromMilliseconds(10)),
            "Bearer test-token"
        );
        HttpResponseMessage response = await wire.SendAsync("GET", "https://api.example.com", []);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(2, attemptCount);
    }

    [TestMethod]
    public async Task ShouldChainMultipleRetryDecorators()
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
        var wire = new RetryWire(
            new RetryWire(mockWire, 1, TimeSpan.FromMilliseconds(5)),
            1,
            TimeSpan.FromMilliseconds(5)
        );
        HttpResponseMessage response = await wire.SendAsync("GET", "https://api.example.com", []);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    public async Task ShouldChainManagedWireWithBasicAuth()
    {
        var handler = new MockHttpMessageHandler(request =>
        {
            Assert.IsTrue(request.Headers.Contains(HttpHeaders.AUTHORIZATION));
            return new HttpResponseMessage(HttpStatusCode.OK);
        });
        var managedWire = new ManagedHttpWire(() => new HttpClient(handler));
        var wire = new BasicAuthWire(managedWire, "Bearer test-token");
        HttpResponseMessage response = await wire.SendAsync("GET", "https://api.example.com", []);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }
}
