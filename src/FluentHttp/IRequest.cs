using FluentHttp.Response;

namespace FluentHttp;

/// <summary>
/// Fluent HTTP request builder.
/// </summary>
public interface IRequest
{
    IRequest Body(string body);
    BaseResponse Fetch();
    Task<BaseResponse> FetchAsync();
    IRequest Header(string name, string value);
    IRequest Method(string method);
    IRequest Through(IWire wire);
    UriBuilder Uri();
}
