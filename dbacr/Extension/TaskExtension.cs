namespace Dbacr.Extension;

public static class TaskExtension
{
    // create a async task extension that try to return a function result
    public static async Task<T?> TryAsync<T>(this Task<T> task, Action<Exception>? exception = null)
    {
        try
        {
            return await task;
        }
        catch(Exception e)
        {
            exception?.Invoke(e);
            return default;
        }
        
    }
    // create a async task extension that try to return a function result
    public static async Task TryAsync(this Task task, Action<Exception>? exception = null)
    {
        try
        {
            await task;
        }
        catch (Exception e)
        {
            exception?.Invoke(e);
        }
    }
}
