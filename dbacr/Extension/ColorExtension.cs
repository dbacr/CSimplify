using System.Security.Cryptography;

namespace Dbacr.Extension;

public static class ColorExtension
{
    // create a extension that converts a html color to RGB Integer
    public static int ToRGBInteger(this string color)
    {
        if (color.StartsWith("#"))
            color = color[1..];

        if (color.Length != 6)
            return 0;

        return int.Parse(color, System.Globalization.NumberStyles.HexNumber);
    }

    // create a method to get random hex colors
    public static string GetRandomHexColor()
      => String.Format("#{0}", RandomNumberGenerator.GetInt32(0, 0x1000000).ToString("X6"));
}
