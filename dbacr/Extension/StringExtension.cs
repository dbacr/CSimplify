using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Dbacr.Extension;

public static class StringExtension
{

    /// <summary>
    /// Extension to check if many strings are null or empty
    /// </summary>
    /// <param name="_"></param>
    /// <param name="datas"></param>
    /// <returns></returns>
    public static bool IsNullOrEmptyMany(this string _, params string?[] datas)
        => datas.Any(a => string.IsNullOrEmpty(a));   
    
    /// <summary>
    /// Extension to trim when string exceed max length
    /// </summary>
    /// <param name="str"></param>
    /// <param name="maxLength"></param>
    /// <returns></returns>
    public static string Limit(this string str, int maxLength)
        => str.Length > maxLength ? str[..maxLength] : str;

    /// <summary>
    /// Extension to return a string if the original is null or empty
    /// </summary>
    /// <param name="data"></param>
    /// <param name="ifNullthen"></param>
    /// <returns></returns>
    public static string IsNullOrEmptyThen(this string data, string ifNullthen)
        => string.IsNullOrEmpty(data) ? ifNullthen : data;

    /// <summary>
    /// Extension to check if a string contains any of the given words
    /// </summary>
    /// <param name="str"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool ContainsAny(this string str, params string[] values)
        => values.Any(str.Contains);
    
    /// <summary>
    /// Extension to pluralize a word
    /// </summary>
    /// <param name="word"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static string Pluralize(this string word, int count)
        => count == 1 ? word : word + "s";
    public static string Pluralize(this string word, double count)
        => count <= 1 ? word : word + "s";
    /// <summary>
    /// Extension to validate if a string is a valid md5 that has a length of 32 or 20 characters
    /// </summary>
    /// <param name="hash"></param>
    /// <returns></returns>
    public static bool IsValidMD5(this string hash)
        => Regex.IsMatch(hash, @"^[a-fA-F0-9]{32}$|^[a-fA-F0-9]{20}$");
    /// <summary>
    /// Extension to hash a string to md5
    /// </summary>
    public static string ToMd5(this string str)
        => BitConverter.ToString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(str))).Replace("-", "");
    /// <summary>
    /// Extension to convert a string to a hex string
    /// </summary>
    public static string ToHex(this string str) => Convert.ToHexString(Encoding.UTF8.GetBytes(str));
    /// <summary>
    /// Extension to convert a hex string to a string
    /// </summary>
    public static string FromHex(this string str) => Encoding.UTF8.GetString(Convert.FromHexString(str));
    /// <summary>
    /// Extension to convert a string to a base64 string
    /// </summary>
    public static string ToBase64(this string str) => Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
    /// <summary>
    /// Extension to convert a base64 string to a string
    /// </summary>
    public static string FromBase64(this string str) => Encoding.UTF8.GetString(Convert.FromBase64String(str));
    /// <summary>
    /// Extension to try to convert a base64 string to a string
    /// </summary>
    public static bool TryFromBase64(this string str, out string? result)
     {
        try {
            result = Encoding.UTF8.GetString(Convert.FromBase64String(str));
            return true;
        } catch {
            result = default;
            return false;
        }
    }
    /// <summary>
    /// Extension to try to convert a hex string to a string
    /// </summary>
    public static bool TryFromHexString(this string str, out string? result)
     {
        try {
            result = Encoding.UTF8.GetString(Convert.FromHexString(str));
            return true;
        } catch {
            result = default;
            return false;
        }
    }
}
