namespace FluentHttp;

public interface IUriBuilder
{
    IRequest Back();
    string Build();
    IUriBuilder Path(string pathSegment);
    IUriBuilder QueryParam(string name, object value);
}
