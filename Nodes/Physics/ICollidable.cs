/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

namespace NoZ.Nodes.Physics
{
    public interface ICollidable
    {
        public void OnCollisionEnter(Collider2D collider);

        public void OnCollisionExit(Collider2D collider);
    }
}
