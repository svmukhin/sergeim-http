using FluentHttp.Response;

namespace FluentHttp;

public interface IResponse
{
    int StatusCode { get; }
    string Content { get; }

    T As<T>() where T : IResponse;
    IResponse AssertStatus(int expectedStatus);
    string? GetHeader(string name);
    IRequest Header(string name, string value);
    IRequest Rel(string href);
}
