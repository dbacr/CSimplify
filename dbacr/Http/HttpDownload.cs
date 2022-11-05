namespace Dbacr.Http;

public class HttpDownload
{
    public Stream? Stream { get; set; }
    public Exception? Exception { get; internal set; }
    public bool IsSuccess { get; internal set; }       

}