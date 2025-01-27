/*

    Copyright (c) 2024 NoZ Games, LLC. All rights reserved.

*/

namespace NoZ
{
    public struct Rect
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;

        public float MinX => X;
        public float MinY => Y;
        public float MaxX => X + Width;
        public float MaxY => Y + Height;

        public Rect(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public static implicit operator Raylib_cs.Rectangle(Rect r) => new Raylib_cs.Rectangle(r.X, r.Y, r.Width, r.Height);
    }
}
