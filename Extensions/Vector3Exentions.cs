/*

    Copyright (c) 2024 NoZ Games, LLC. All rights reserved.

*/

using System.Numerics;

namespace NoZ
{
    public static class Vector3Extensions
    {
        public static Vector2 XY(this Vector3 v) => new Vector2(v.X, v.Y);
        public static Vector3 X_Z(this Vector3 v) => new Vector3(v.X, 0, v.Z);
        public static Vector2 XZ(this Vector3 v) => new Vector2(v.X, v.Z);
    }
}
