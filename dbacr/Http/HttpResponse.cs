
using System.Net;

namespace Dbacr.Http;
public class HttpResponse<T>
{
    public Exception? Exception { get; internal set; }
    public HttpRequestMessage? Request { get; internal set; }
    public HttpResponseMessage? Response { get; internal set; }
    public T? Body { get; internal set; }
    public bool IsSuccess { get; internal set; }
    public string? Raw { get; internal set; }
    public Stream? Stream { get; internal set; }
    public HttpStatusCode? StatusCode
           => Response?.StatusCode;

    public HttpResponse() { }
    public HttpResponse(HttpResponseMessage msg)
    {
        Request = msg.RequestMessage;
        Response = msg;
    }

}

