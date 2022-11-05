namespace Dbacr.Extension;

public static class TryExtension
{
    /// <summary>
    /// Try to execute a function otherwise it will return null
    /// </summary>
    public static T? Try<T>(this T obj, Func<T, T> func)
    {
        try
        {
            return func(obj);
        }
        catch (Exception)
        {
            return default;
        }
    }

    /// <summary>
    /// Try to execute a function otherwise it will return null
    /// </summary>
    public static T? Try<T>(this Func<T> func)
    {
        try
        {
            return func();
        }
        catch (Exception)
        {
            return default;
        }
    }

}
