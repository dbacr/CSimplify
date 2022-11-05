using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using Dbacr.Extension;
using Polly;
using Polly.Contrib.WaitAndRetry;

namespace Dbacr.Http;

public static class Http
{
    public static readonly IAsyncPolicy<HttpResponseMessage> RetryPolicy = Policy<HttpResponseMessage>
        .Handle<HttpRequestException>()
        .OrResult(r => r.StatusCode is >= HttpStatusCode.InternalServerError or HttpStatusCode.RequestTimeout)
        .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1), retryCount: 5));

    /// <summary>
    ///     Gets or creates a new HttpClient with the default headers.
    /// </summary>
    public static HttpClient GetClient(WebProxy? proxy = null)
         => proxy is null ? new HttpClient() : new HttpClient(new HttpClientHandler { Proxy = proxy });

    /// <summary>
    /// Http method to fetch the content length of a remote resource.
    /// </summary>
    private static async Task<long> GetContentLengthAsync(string url)
    {
        var response = await GetClient().SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
        return response.Content.Headers.ContentLength ?? 0;
    }
    
    /// <summary>
    /// Http method to download a file from a url and optionally save it to disk
    /// </summary>
    public static async Task<HttpDownload> TryDownloadAsync(Action<HttpDownloadConfig> config, Action<(long Current, long Total, string? percentage)>? progress = null)
    {
        var download = new HttpDownload();
        try
        {
            var cfg = config.GetAction();
            download.Stream =  await GetClient().GetStreamAsync(cfg.Url);
            download.IsSuccess = true;
            
            if (cfg.SaveToDisk && !string.IsNullOrEmpty(cfg.SavePath))
            {
                if (cfg.WithProgress)
                {
                    var contentLength = await GetContentLengthAsync(cfg.Url!);

                    await Task.Factory.StartNew(async () => {
                        var timer = new PeriodicTimer(new TimeSpan(0, 0, 0, 0, 1));
                        while (await timer.WaitForNextTickAsync())
                        {
                            var fileInfo = new FileInfo(cfg.SavePath);
                            if (!fileInfo.Exists)
                                continue;
                            
                            progress?.Invoke((
                                fileInfo.Length, 
                                contentLength, 
                                Math.Floor((decimal)fileInfo.Length / contentLength * 100).ToString(CultureInfo.InvariantCulture)));
                        }
                    
                        timer.Dispose();    
                    });
                }
                
                await using var fileStream = File.Create(cfg.SavePath);
                await download.Stream.CopyToAsync(fileStream);
                await download.Stream.DisposeAsync();
            }

        }
        catch (HttpRequestException ex)
        {
            download.Exception = ex;
        }

        return download;
    }

    /// <summary>
    /// Http method that sends a http request
    /// you can specify the method, url, headers and body in the config
    /// returns a object
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    public static async Task<HttpResponse<object>> TrySendAsync(Action<HttpConfig> config)
        => await TrySendAsync<object>(config);


    /// <summary>
    /// Http method that trys to get a string async
    /// </summary>
    /// <returns></returns>
    public static async Task<string?> TryGetStringAsync(string url)
        => await GetClient().GetStringAsync(url).TryAsync();

    /// <summary>
    /// Http method that trys to get a stream
    /// </summary>
    /// <returns></returns>
    public static async Task<HttpResult<Stream?>> TryGetStreamAsync(string url)
    {
        var http = new HttpResult<Stream?>();
        var val = await GetClient().GetAsync(url).TryAsync(a => http.Exception = a as HttpRequestException);

        http.Request = val?.RequestMessage;
        http.Result =
            val?.IsSuccessStatusCode ?? false
                ? await val.Content.ReadAsStreamAsync()
                : null;
        return http;
    }


    /// <summary>
    /// Http method that sends a http request
    /// you can specify the method, url, headers and body in the config
    /// deserialize the response to the specified type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="config"></param>
    /// <returns></returns>
    public static async Task<HttpResponse<T>> TrySendAsync<T>(Action<HttpConfig> config)
    {
        var cfg = config.GetAction();
        var request = new HttpRequestMessage(cfg.Method, cfg.Url)
        {
            Content = cfg.Content
        };
        var response = new HttpResponse<T>
        {
            Request = request
        };

        // set auth header 
        if (cfg.AuthToken != null)
            request.Headers.Authorization = new AuthenticationHeaderValue(cfg.AuthType ?? "Bearer", cfg.AuthToken);

        // set content type header 
        if (cfg.ContentType != null && request.Content != null)
            request.Content.Headers.ContentType = new MediaTypeHeaderValue(cfg.ContentType.GetContentType());

        // set referer header
        if (cfg.Referer != null)
            request.Headers.Referrer = new Uri(cfg.Referer);

        // set accept header
        if (cfg.Accept != null)
            request.Headers.Accept.TryParseAdd(cfg.Accept);

        // set encoding header
        if (cfg.Encoding != null)
            request.Headers.AcceptEncoding.TryParseAdd(cfg.Encoding);

        // set user-agent
        if (cfg.Agent != null)
            request.Headers.UserAgent.TryParseAdd(cfg.Agent);

        try
        {

            // make a request
            response.Response= await RetryPolicy.ExecuteAsync(async () => await GetClient(cfg.Proxy).SendAsync(request.Clone()));
            response.IsSuccess = response.Response.IsSuccessStatusCode;
            
            // if the response is success then read the response
            if (response.IsSuccess)
            {
                response.Body = await response.Response.Content.ReadAsAsync<T>();
                response.Raw = await response.Response.Content.ReadAsStringAsync();
                response.Stream = await response.Response.Content.ReadAsStreamAsync();
            }
            else
                response.Exception = new HttpRequestException(response.Response.ReasonPhrase);

            return response;
        }
        catch (Exception ex)
        {
            response.Exception = ex;
            return response;
        }
    }
}
