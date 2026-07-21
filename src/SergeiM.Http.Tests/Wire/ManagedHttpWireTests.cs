// SPDX-FileCopyrightText: Copyright (c) [2025-2026] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using System.Net;
using SergeiM.Http.Tests.Wire.Mocks;
using SergeiM.Http.Wire;

namespace SergeiM.Http.Tests.Wire;

[TestClass]
public class ManagedHttpWireTests : WireTestBase
{
    protected override IWire CreateWire(HttpClient client)
    {
        return new ManagedHttpWire(() => client);
    }

    [TestMethod]
    public void Constructor_ShouldAcceptFactoryDelegate()
    {
        var wire = new ManagedHttpWire(() => new HttpClient());
        Assert.IsNotNull(wire);
    }

    [TestMethod]
    public async Task SendAsync_ShouldCreateNewClientPerRequest()
    {
        int createCount = 0;
        var handler = new MockHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
        var wire = new ManagedHttpWire(() =>
        {
            createCount++;
            return new HttpClient(handler);
        });
        _ = await wire.SendAsync("GET", "https://api.example.com", []);
        _ = await wire.SendAsync("GET", "https://api.example.com", []);
        Assert.AreEqual(2, createCount);
    }
}
