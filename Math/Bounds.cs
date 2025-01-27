/*

    Copyright (c) 2024 NoZ Games, LLC. All rights reserved.

*/

using System.Numerics;

namespace NoZ
{
    public struct Bounds
    {
        public Vector3 Center;
        public Vector3 Size;
        
        public Vector3 Min => Center - Size * 0.5f;
        public Vector3 Max => Center + Size * 0.5f;
        
        public Bounds(Vector3 center, Vector3 size)
        {
            Center = center;
            Size = size;
        }
    }
}