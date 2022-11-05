using System.Net;

namespace Dbacr.Http;

public class HttpConfig
{
    public string? Url { get; set; }
    public string? AuthToken { get; set; }
    public string? AuthType { get; set; }
    public string? Referer { get; set; }
    public string? Accept { get; set; }
    public string? Encoding { get; set; }
    public string? Agent { get; set; }
    public WebProxy? Proxy { get; set; }
    public HttpContent? Content { get; set; }
    public HttpContentType? ContentType { get; set; }
    public HttpMethod Method { get; set; }
        = HttpMethod.Get;
}