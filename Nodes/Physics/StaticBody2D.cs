/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

namespace NoZ.Nodes.Physics
{
    public class StaticBody2D : RigidBody2D
    {
        public StaticBody2D()
        {
            IsStatic = true;
        }
    }
}
