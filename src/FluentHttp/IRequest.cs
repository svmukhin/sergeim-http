namespace FluentHttp;

/// <summary>
/// Fluent HTTP request builder.
/// </summary>
public interface IRequest
{
    Request Body(string body);
    BaseResponse Fetch();
    Task<BaseResponse> FetchAsync();
    Request Header(string name, string value);
    Request Method(string method);
    UriBuilder Uri();
}
