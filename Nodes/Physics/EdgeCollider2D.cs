/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

using System.Numerics;
using Box2D.NetStandard.Collision.Shapes;

namespace NoZ.Nodes.Physics
{
    public class EdgeCollider2D : Collider2D
    {
        private Vector2 _v1;
        private Vector2 _v2;

        public EdgeCollider2D (Vector2 v1, Vector2 v2, ushort collisionCategory=1, ushort collisionMask=0xFFFF)
        {
            _v1 = v1;
            _v2 = v2;
            CollisionCategory = collisionCategory;
            CollisionMask = collisionMask;
        }

        protected override Shape CreateShape() =>
            new EdgeShape(_v1, _v2 );
    }
}
