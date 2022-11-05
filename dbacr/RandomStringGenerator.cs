using System.Security.Cryptography;

namespace Dbacr;

public static class RandomStringGenerator
{
    
    /// <summary>
    /// Generate random string based on length
    /// </summary>
    public static string GetString(int length)
        => GetString(length, false);

    /// <summary>
    /// Generate random string based on random length
    /// </summary>
    public static string GetString(bool isRandom)
        => GetString(20, isRandom);
    
    /// <summary>
    /// Generate random string
    /// </summary>
    private static string GetString(int length = 20, bool isRandom = false)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, isRandom ? RandomNumberGenerator.GetInt32(10,500) : length)
            .Select(s => s[RandomNumberGenerator.GetInt32(s.Length)]).ToArray());
    }
    
}