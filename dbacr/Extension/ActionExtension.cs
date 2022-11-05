namespace Dbacr.Extension;

public static class ActionExtension
{
    /// <summary>
    /// Extension to get the typeargument from a <see cref="Action<T>"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="action"></param>
    /// <returns></returns>
    public static T GetAction<T>(this Action<T> action)
    {
        var result = Activator.CreateInstance<T>();

        action(result);

        return result;
    }
}
