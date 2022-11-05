namespace Dbacr.Extension;

public static class NullExtension
{
    /// <summary>
    /// Extension to check if a string is null or empty then enter the values
    /// </summary>
    /// <param name="data"></param>
    /// <param name="ifnull"></param>
    /// <param name="elsedo"></param>
    /// <returns></returns>
    public static string IfNullThen(this object? data, string ifnull, string elsedo)
        => data is null ? ifnull : elsedo;
}
