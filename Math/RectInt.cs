/*

    Copyright (c) 2024 NoZ Games, LLC. All rights reserved.

*/

using Raylib_cs;

namespace NoZ
{
    public struct RectInt
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public int MinX => X;
        public int MinY => Y;
        public int MaxX => X + Width;
        public int MaxY => Y + Height;
        
        public RectInt(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
        
        public RectInt Shrink(int amount) => new(X + amount, Y + amount, Width - amount * 2, Height - amount * 2);
        
        public static implicit operator Rect(RectInt rect) => new(rect.X, rect.Y, rect.Width, rect.Height);
    }
}