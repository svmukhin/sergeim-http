using System.Net;
using FluentHttp.Response;

namespace FluentHttp.Tests;

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

    private static HttpResponseMessage CreateMockXmlResponse(string xml)
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(xml, System.Text.Encoding.UTF8, "text/xml")
        };
        return response;
    }
}
