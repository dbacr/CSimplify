namespace Dbacr.Http;

public class HttpResult<T> : HttpDefault
{
    public T? Result { get; set; }
}