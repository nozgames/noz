/*

    Copyright (c) 2024 NoZ Games, LLC. All rights reserved.

*/

using System.Numerics;

namespace NoZ
{
    public static class Randomizer
    {
        private static System.Random s_random;

        static Randomizer()
        {
            s_random = new System.Random(DateTime.UtcNow.Millisecond);
        }

        public static float Next() => Range(0.0f, 1.0f);

        public static float Next(int size) => s_random.Next(size);
        
        public static int Range(int min, int max) => s_random.Next(min, max);
        
        public static float Range(float min, float max) => s_random.Next(0, 100000) / 100000.0f * (max - min) + min;
        
        public static Vector3 Range(in Vector3 min, in Vector3 max) =>
            new(Range(min.X, max.X), Range(min.Y, max.Y), Range(min.Z, max.Z));

        public static Vector2 Range(in Vector2 min, in Vector2 max) =>
            new(Range(min.X, max.X), Range(min.Y, max.Y));

        public static Vector2Int Range(in Vector2Int min, in Vector2Int max) =>
            new(Range(min.X, max.X), Range(min.Y, max.Y));

    }
}
            
