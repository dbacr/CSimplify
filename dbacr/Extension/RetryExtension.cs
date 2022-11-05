namespace Dbacr.Extension;

public static class RetryExtension
{
    /// <summary>
    /// Retry a function until it succeed or the max retry is reached
    /// </summary>
    public static async Task<T> RetryAsync<T>(this Task<T> task, int retryCount = 3)
    {
        for (var i = 0; i < retryCount; i++)
        {
            try
            {
                return await task;
            }
            catch
            {
                await Task.Delay(500);
            }
        }
        return await task;
    }
 

    /// <summary>
    /// Retry a function until it succeed or the max retry is reached
    /// </summary>
    public static async Task RetryAsync(this Task task, int retryCount = 3)
    {
        for (var i = 0; i < retryCount; i++)
        {
            try
            {
                await task;
            }
            catch
            {
                await Task.Delay(500);
            }
        }
        await task;
    }
}
