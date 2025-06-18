/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

using System.Numerics;

namespace NoZ.Graphics
{
    internal struct RenderObject2D
    {
        public Mesh Mesh;
        public int MaterialIndex;
        public Matrix3x2 Transform;        
        public int SortOrder;

        // TODO: Remove Raylib_cs dependencies and implement SDL3 rendering logic as needed.
    }
}
