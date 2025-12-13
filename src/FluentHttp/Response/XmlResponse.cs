using System.Xml;
using System.Xml.XPath;

namespace FluentHttp.Response;

/// <summary>
/// HTTP response with XML parsing capabilities.
/// </summary>
public class XmlResponse : BaseResponse
{
    private XmlDocument? _xmlDocument;
    private XPathNavigator? _navigator;

    public XmlResponse(HttpResponseMessage response) : base(response)
    {
    }

    /// <summary>
    /// Gets the parsed XML document.
    /// </summary>
    public XmlDocument XmlDocument
    {
        get
        {
            if (_xmlDocument == null)
            {
                _xmlDocument = new XmlDocument();
                _xmlDocument.LoadXml(Content);
            }
            return _xmlDocument;
        }
    }

    /// <summary>
    /// Gets the XPath navigator for the document.
    /// </summary>
    public XPathNavigator Navigator
    {
        get
        {
            _navigator ??= XmlDocument.CreateNavigator()!;
            return _navigator;
        }
    }

    /// <summary>
    /// Asserts that an XPath expression matches at least one node.
    /// </summary>
    public XmlResponse AssertXPath(string xpath)
    {
        _ = Navigator.SelectSingleNode(xpath) ?? throw new InvalidOperationException(
                $"XPath expression '{xpath}' did not match any nodes in the XML response.");
        return this;
    }

    /// <summary>
    /// Evaluates an XPath expression and returns the result as a string.
    /// </summary>
    public string EvaluateXPath(string xpath)
    {
        var result = Navigator.Evaluate(xpath);
        if (result is XPathNodeIterator iterator)
        {
            if (iterator.MoveNext())
            {
                return iterator.Current?.Value ?? string.Empty;
            }
            throw new InvalidOperationException($"XPath expression '{xpath}' did not return any value.");
        }
        return result?.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Creates a new request following a link from an XPath expression.
    /// </summary>
    public new IRequest Rel(string xpath)
    {
        var href = EvaluateXPath(xpath);
        return base.Rel(href);
    }
}
