/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

using Box2D.NetStandard.Collision.Shapes;
using Box2D.NetStandard.Dynamics.Fixtures;

namespace NoZ.Nodes.Physics
{
    public abstract class Collider2D : Node
    {
        private Fixture? _fixture;
        private RigidBody2D? _rigidBody;
        private ICollidable? _collidable;
        private ushort _collisionMask;
        private ushort _collisionCategory;

        public RigidBody2D RigidBody => _rigidBody!;

        public ushort CollisionMask
        {
            get => _collisionMask;
            set
            {
                _collisionMask = value;
                if (_fixture != null)
                    _fixture.FilterData.maskBits = value;
            }
        }

        public ushort CollisionCategory
        {
            get => _collisionCategory;
            set
            {
                _collisionCategory = value;
                if (_fixture != null)
                    _fixture.FilterData.categoryBits = _collisionCategory;
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            _collidable = null;
        }

        protected override void OnEnterWorld()
        {
            // Must be parented to a rigid body
            _rigidBody = GetParent<RigidBody2D>();

            // Check if collision is being handled
            _collidable = _rigidBody.Parent as ICollidable;

            // Create the fixture
            var fixtureDef = new FixtureDef();
            fixtureDef.shape = CreateShape();
            fixtureDef.friction = 0.01f;
            fixtureDef.restitution = 0.1f;
            fixtureDef.density = 1.0f;
            fixtureDef.filter = new Filter();
            fixtureDef.filter.groupIndex = 0;
            fixtureDef.filter.categoryBits = _collisionCategory;
            fixtureDef.filter.maskBits = _collisionMask;
            fixtureDef.userData = this;
            _rigidBody._body!.CreateFixture(fixtureDef);

            base.OnEnterWorld();            
        }

        protected override void OnExitWorld()
        {
            base.OnExitWorld();

            if (_rigidBody != null)
            {
                _rigidBody._body!.DestroyFixture(_fixture);
                _rigidBody = null;
                _fixture = null;
            }
        }

        protected abstract Shape CreateShape();

        internal void OnCollisionEnter(Collider2D collider)
        {
            _collidable?.OnCollisionEnter(collider);
        }

        internal void OnCollisionExit(Collider2D collider)
        {
            _collidable?.OnCollisionExit(collider);
        }
    }
}
