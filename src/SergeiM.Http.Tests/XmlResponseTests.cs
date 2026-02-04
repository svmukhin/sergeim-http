// SPDX-FileCopyrightText: Copyright (c) [2025-2026] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using System.Net;
using SergeiM.Http.Response;

namespace SergeiM.Http.Tests;

[TestClass]
public class XmlResponseTests
{
    [TestMethod]
    public void XmlResponse_ShouldParseXml()
    {
        var xml = """
            <?xml version="1.0"?>
            <page>
                <title>Test Page</title>
                <links>
                    <link rel="see" href="/api/details/123"/>
                    <link rel="edit" href="/api/edit/123"/>
                </links>
            </page>
            """;
        var title = new XmlResponse(CreateMockXmlResponse(xml)).EvaluateXPath("/page/title");
        Assert.AreEqual("Test Page", title);
    }

    [TestMethod]
    public void XmlResponse_ShouldAssertXPath()
    {
        var xml = """
            <?xml version="1.0"?>
            <page>
                <links>
                    <link rel="see" href="/api/details"/>
                </links>
            </page>
            """;
        new XmlResponse(CreateMockXmlResponse(xml)).AssertXPath("/page/links/link[@rel='see']");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void XmlResponse_ShouldThrowOnMissingXPath()
    {
        var xml = """
            <?xml version="1.0"?>
            <page>
                <title>Test</title>
            </page>
            """;
        new XmlResponse(CreateMockXmlResponse(xml)).AssertXPath("/page/nonexistent");
    }

    [TestMethod]
    public void XmlResponse_ShouldEvaluateXPathAttribute()
    {
        var xml = """
            <?xml version="1.0"?>
            <page>
                <links>
                    <link rel="see" href="/api/details/456"/>
                </links>
            </page>
            """;
        var href = new XmlResponse(CreateMockXmlResponse(xml)).EvaluateXPath("/page/links/link[@rel='see']/@href");
        Assert.AreEqual("/api/details/456", href);
    }

    [TestMethod]
    public void XmlResponse_AssertStatus_ShouldReturnXmlResponse()
    {
        var xml = """
            <?xml version="1.0"?>
            <result>success</result>
            """;
        var response = new XmlResponse(CreateMockXmlResponse(xml));
        var result = response.AssertStatus(200).EvaluateXPath("/result");
        Assert.AreEqual("success", result);
    }

    [TestMethod]
    public void XmlResponse_AssertStatus_AfterAs_ShouldAllowChaining()
    {
        var xml = """
            <?xml version="1.0"?>
            <data><value>test</value></data>
            """;
        var httpResponse = CreateMockXmlResponse(xml);
        var baseResponse = new BaseResponse(httpResponse);
        var result = baseResponse.As<XmlResponse>().AssertStatus(200).EvaluateXPath("/data/value");
        Assert.AreEqual("test", result);
    }

    private static HttpResponseMessage CreateMockXmlResponse(string xml)
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(xml, System.Text.Encoding.UTF8, "text/xml")
        };
        return response;
    }
}
