/*

    Copyright (c) 2024 NoZ Games, LLC. All rights reserved.

*/

using NoZ.Graphics;

namespace NoZ
{
    public static class ColorExtensions
    {
        public static Color MultiplyAlpha(this Color c, float a) => new Color(c.R, c.G, c.B, (byte)(c.A * a));
    }
}
