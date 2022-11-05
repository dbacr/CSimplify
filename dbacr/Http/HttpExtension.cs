namespace Dbacr.Http;

internal static class HttpExtension
{
    /// <summary>
    /// Clones a HttpRequestMessage
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public static HttpRequestMessage Clone(this HttpRequestMessage req)
    {
        HttpRequestMessage clone
            = new(req.Method, req.RequestUri);

        clone.Content = req.Content;
        clone.Version = req.Version;

        foreach (var prop in req.Options) clone.Options.TryAdd(prop.Key, prop.Value);
        foreach (var header in req.Headers) clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

        return clone;
    }

    /// <summary>
    /// Reads the http-content as type-argument
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="content"></param>
    /// <returns></returns>
    public static async Task<T?> ReadAsAsync<T>(this HttpContent content)
    {
        var json = await content.ReadAsStringAsync();
        return json.TryDeserializeJson<T>();
    }
}