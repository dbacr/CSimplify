namespace Dbacr.Http;

public class HttpDownloadConfig
{
    public bool SaveToDisk { get; set; }
    public bool WithProgress { get; set; }
    public string? SavePath { get; set; }
    public string? Url { get; set; }
        
}