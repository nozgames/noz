/*
  NoZ Game Engine

  Copyright(c) 2019 NoZ Games, LLC

  Permission is hereby granted, free of charge, to any person obtaining a copy
  of this software and associated documentation files(the "Software"), to deal
  in the Software without restriction, including without limitation the rights
  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
  copies of the Software, and to permit persons to whom the Software is
  furnished to do so, subject to the following conditions :

  The above copyright notice and this permission notice shall be included in all
  copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
  SOFTWARE.
*/

namespace NoZ.Physics
{
    public abstract class Collider : PhysicsNode
    {
        /// <summary>
        /// Body used for colliders not parented to a RigidBody
        /// </summary>
        private IBody _body;

        /// <summary>
        /// Collider
        /// </summary>
        private ICollider _collider;

        private int _layer;

        public int Layer {
            get => _layer;
            set {
                _layer = value;
                if(_collider != null)
                {
                    _collider.CollisionMask = (uint)(1 << _layer);
                    _collider.CollidesWithMask = Physics.GetLayerCollisionMask(_layer);
                }
            }
        }

        protected override void OnAnscestorChanged()
        {
            base.OnAnscestorChanged();

            // Make sure we are parented to a body.
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            // TODO: scan through parents for a rigid body node and use its body instead

            RigidBody parentBody = null;
            for (var parent = Parent; parentBody == null && parent != null; parent = parent.Parent)
                parentBody = parent as RigidBody;

            if (parentBody == null)
            {
                // Create a new body
                _body = Scene.World.CreateStaticBody();
                _collider = CreateCollider(_body);
            }
            else
            {
                parentBody.Enable();
                _collider = CreateCollider(parentBody._body);
            }

            if (_collider == null)
            {
                Disable();
                return;
            }

            _collider.CollisionMask = (uint)(1 << _layer);
            _collider.CollidesWithMask = Physics.GetLayerCollisionMask(_layer);
            _collider.Node = this;
        }

        protected override void OnDisable ()
        {
            _collider?.Dispose();
            _collider = null;

            _body?.Dispose();
            _body = null;
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            if(_body != null)
                _body.Position = Physics.PixelsToMeters(LocalToWorld.MultiplyVector(Vector2.Zero));
        }

        protected abstract ICollider CreateCollider(IBody body);
    }
}
