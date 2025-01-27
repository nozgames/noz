/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

using System.Numerics;
using Box2D.NetStandard.Collision.Shapes;
using Box2D.NetStandard.Dynamics.Bodies;

namespace NoZ.Nodes
{
    /// <summary>
    /// Adds physics to the parent Node2D
    /// </summary>
    public class RigidBody2D : Node, IFixedUpdate
    {
        internal Body? _body = null;
        private Vector2 _linearVelocity;

        public bool IsStatic { get; set; }

        public float Acceleration { get; set; } = 1;
        public float Friction { get; set; } = 0;
        public Vector2 LinearVelocity
        {
            get => _linearVelocity;
            set
            {
                _linearVelocity = value;
                _body?.SetLinearVelocity(_linearVelocity);
            }
        }

        protected override void OnEnterWorld()
        {
            var parent = GetParent<Node2D>();

            var bodyDef = new BodyDef()
            {
                type = IsStatic
                    ? BodyType.Static
                    : BodyType.Dynamic,
                position = parent.Position,
                enabled = true,
                userData = this,
                awake = true,
                allowSleep = false
            };

            _body = Scene!.Physics.CreateBody(bodyDef);
            _body.SetAwake(true);

            if (!IsStatic)
                _body.SetSleepingAllowed(false);

            if (_linearVelocity.X != 0 || _linearVelocity.Y != 0)
                _body.SetLinearVelocity(_linearVelocity);

            base.OnEnterWorld();
        }

        protected override void OnExitWorld()
        {
            if (null != _body)
            {
                Scene!.Physics.DestroyBody(_body);
                _body = null;
            }

            base.OnExitWorld();
        }

        void IFixedUpdate.FixedUpdate()
        {
            if (Parent is not Node2D parent)
                return;

            if (_body == null)
                return;


            //var resolvedFriction = MathF.Min(Friction * Time.DeltaTime, 1.0f);
            //Velocity -= Velocity * resolvedFriction;

            //parent.LocalPosition += Velocity * Time.DeltaTime;
            _linearVelocity = _body.GetLinearVelocity();
            parent.Position = _body.GetPosition();
        }
    }
}
