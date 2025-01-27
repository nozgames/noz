/*

    Copyright (c) 2024 NoZ Games, LLC. All rights reserved.

*/

using Raylib_cs;

namespace NoZ.Helpers
{
    public static class ColorHelper
    {
        public static Color ParseColor(string value)
        {
            // Parse the hex string into a color
            if (!value.StartsWith('#'))
                return Color.White;

            var hex = value[1..];
            if (hex.Length == 3)
            {
                var r = Convert.ToByte(hex[..1], 16);
                var g = Convert.ToByte(hex.Substring(1, 1), 16);
                var b = Convert.ToByte(hex.Substring(2, 1), 16);
                return new Color(r + (r << 4), g + (g << 4), b + (b << 4), (byte)255);
            }
            else if (hex.Length == 6)
            {
                var r = Convert.ToByte(hex[..2], 16);
                var g = Convert.ToByte(hex.Substring(2, 2), 16);
                var b = Convert.ToByte(hex.Substring(4, 2), 16);
                return new Color(r, g, b, (byte)255);
            }
            else if (hex.Length == 8)
            {
                var r = Convert.ToByte(hex[..2], 16);
                var g = Convert.ToByte(hex.Substring(2, 2), 16);
                var b = Convert.ToByte(hex.Substring(4, 2), 16);
                var a = Convert.ToByte(hex.Substring(6, 2), 16);
                return new Color(r, g, b, a);
            }

            return Color.White;
        }

        public static Color Lerp(Color a, Color b, float t)
        {
            return new Color(
                (byte)(a.R + (b.R - a.R) * t),
                (byte)(a.G + (b.G - a.G) * t),
                (byte)(a.B + (b.B - a.B) * t),
                (byte)(a.A + (b.A - a.A) * t));
        }
    }
}
