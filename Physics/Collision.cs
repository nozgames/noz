using System;
using System.Collections.Generic;
using System.Text;

namespace NoZ
{
    public class Collision
    {
        /// <summary>
        /// The incoming collider involved in the collision
        /// </summary>
        public Collider Collider { get; set; }

        /// <summary>
        /// The other collider involved in the collision
        /// </summary>
        public Collider OtherCollider { get; set; }
    }
}
