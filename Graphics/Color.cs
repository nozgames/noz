/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

namespace NoZ.Graphics 
{
    public struct Color 
    {
        public static readonly Color White = new(255, 255, 255, 255);

        public byte R;
        public byte G;
        public byte B;
        public byte A;
        
        public Color(byte r, byte g, byte b, byte a = 255) 
        {
            R = r; G = g; B = b; A = a;
        }

        public static Color ParseColor(string value)
        {
            // Parse the hex string into a color
            if (!value.StartsWith('#'))
                return White;

            var hex = value[1..];
            if (hex.Length == 3)
            {
                var r = Convert.ToByte(hex[..1], 16);
                var g = Convert.ToByte(hex.Substring(1, 1), 16);
                var b = Convert.ToByte(hex.Substring(2, 1), 16);
                return new Color((byte)(r + (r << 4)), (byte)(g + (g << 4)), (byte)(b + (b << 4)), (byte)255);
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
