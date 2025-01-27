/*

    Copyright (c) 2024 NoZ Games, LLC. All rights reserved.

*/

namespace NoZ
{
    public struct Vector2Int
    {
        public static readonly Vector2Int Zero = new(0, 0);
        public static readonly Vector2Int One = new(1, 1);
        
        public int X;
        public int Y;
        
        public Vector2Int(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Vector2Int Min(in Vector2Int a, in Vector2Int b) =>
            new(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));

        public static Vector2Int Max(in Vector2Int a, in Vector2Int b) =>
            new(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
        
        public static Vector2Int operator+ (in Vector2Int a, in Vector2Int b) =>
            new(a.X + b.X, a.Y + b.Y);

        public static Vector2Int operator -(in Vector2Int a, in Vector2Int b) =>
            new(a.X - b.X, a.Y - b.Y);

        public static Vector2Int operator* (in Vector2Int a, in int b) =>
            new(a.X * b, a.Y * b);

        public static bool operator== (in Vector2Int a, in Vector2Int b) =>
            a.X == b.X && a.Y == b.Y;

        public static bool operator!=(in Vector2Int a, in Vector2Int b) =>
            a.X != b.X || a.Y != b.Y;

        public override bool Equals(object? obj)
        {
            // Don't use this
            throw new NotImplementedException();
        }

        public override int GetHashCode() => HashCode.Combine(X, Y);
    }
}