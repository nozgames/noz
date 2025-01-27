/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

using Box2D.NetStandard.Collision.Shapes;
using System.Numerics;

namespace NoZ.Nodes.Physics
{
    public class CircleCollider2D : Collider2D
    {
        public float Radius { get; private set; }

        public Vector2 Center { get; private set; }

        public CircleCollider2D(float radius)
        {
            Radius = radius;
        }

        public CircleCollider2D(Vector2 position, float radius)
        {
            Radius = radius;
            Center = position;
        }

        protected override Shape CreateShape() =>
            new CircleShape() { Radius = Radius, Center = Center };
    }
}
