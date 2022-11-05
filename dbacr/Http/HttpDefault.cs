namespace Dbacr.Http;

public class HttpDefault
{
    public HttpRequestMessage? Request { get; internal set; }
    public HttpRequestException? Exception { get; internal set; }
    public bool IsSuccess { get; internal set; }

}

