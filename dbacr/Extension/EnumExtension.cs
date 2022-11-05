namespace Dbacr.Extension;

public static class EnumExtension
{
    // a extension method that gets the enum int value
    public static int GetValue(this Enum value)
            => Convert.ToInt32(value);

    // a extension method that gets a random enum value from the enum type T with linq
    public static T GetRandom<T>()
         => Enum.GetValues(typeof(T)).Cast<T>().ToList().Shuffle().First();
    
    // a extension that gets the name of a enum value
    public static string? GetName(this Enum value)
         => Enum.GetName(value.GetType(), value);
}