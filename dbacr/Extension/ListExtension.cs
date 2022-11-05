using System.Security.Cryptography;

namespace Dbacr.Extension;

public static class ListExtension
{
    // create a extension that checks if a list contains many values using linq
    public static bool ContainsMany<T>(this IEnumerable<T> list, params T[] values)
       => values.Any(list.Contains);
    
    /// <summary>
    /// Extension to randomize a list of items
    /// </summary>
    public static List<T> Shuffle<T>(this List<T> list)
        => list.OrderBy(x => RandomNumberGenerator.GetInt32(0, list.Count)).ToList();

    /// <summary>
    /// Extension method to try to replace an item in a list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="oldItem"></param>
    /// <param name="newItem"></param>
    /// <returns></returns>
    public static bool TryReplace<T>(this List<T> list, T oldItem, T newItem)
    {
        var index = list.IndexOf(oldItem);
        if (index != -1)
            list[index] = newItem;

       return index != -1;
    }
}