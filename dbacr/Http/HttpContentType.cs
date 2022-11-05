namespace Dbacr.Http;

public class HttpContentType
{
    private readonly string _value;
    private HttpContentType(string value)
        => _value = value;

    public string GetContentType()
        => _value;
    public static HttpContentType ApplicationJson = new("application/json");
    public static HttpContentType ApplicationXml = new("application/xml");
    public static HttpContentType ApplicationXProtobuffer = new("application/x-protobuffer");
    public static HttpContentType ApplicationFormUrlEncoded = new("application/x-www-form-urlencoded");
    public static HttpContentType TextPlain = new("text/plain");
    public static HttpContentType TextXml = new("text/xml");
    public static HttpContentType TextHtml = new("text/html");
    public static HttpContentType ImageJpeg = new("image/jpeg");
    public static HttpContentType ImagePng = new("image/png");
    public static HttpContentType ImageGif = new("image/gif");
    public static HttpContentType ImageBmp = new("image/bmp");
    public static HttpContentType ImageTiff = new("image/tiff");
    public static HttpContentType ImageSvg = new("image/svg+xml");
    public static HttpContentType ImageVndWapWbmp = new("image/vnd.wap.wbmp");
    public static HttpContentType ImageWebp = new("image/webp");
    public static HttpContentType MultipartFormData = new("multipart/form-data");
    public static HttpContentType TextCss = new("text/css");
    public static HttpContentType TextJavaScript = new("text/javascript");
}