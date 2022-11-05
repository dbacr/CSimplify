namespace Dbacr.Http;

public class HttpDefaultList<T> : List<T> 
{
    public HttpRequestMessage? Request { get; internal set; }
    public Exception? Exception { get; internal set; }
    public bool IsSuccess { get; internal set; }
}