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

namespace NoZ
{
    public class RigidBody : PhysicsNode
    {
        internal IBody _body;
        private Vector2 _linearVelocity;
        private float _linearDamping;
        private bool _sensor;
        private bool _kinematic;

        public bool IsBullet { get; set; } = false;

        public bool IsSensor {
            get => _sensor;
            set {
                _sensor = value;
                if (_body != null)
                    _body.IsSensor = _sensor;
            }
        }

        public bool IsKinematic {
            get => _kinematic;
            set {
                _kinematic = value;
                if (_body != null)
                    _body.IsKinematic = _kinematic;
            }
        }

        public Vector2 LinearVelocity {
            get => _linearVelocity;
            set {
                _linearVelocity = value;
                if (_body != null)
                    _body.LinearVelocity = Physics.PixelsToMeters(_linearVelocity);
            }
        }

        public float LinearDamping {
            get => _linearDamping;
            set {
                _linearDamping = value;
                if(_body != null)
                    _body.LinearDamping = value;
            }
        }

        public void ApplyForce(in Vector2 force) => _body?.ApplyForce(Physics.PixelsToMeters(force));

        protected override void OnEnable()
        {
            base.OnEnable();

            _body = Scene.World.CreateRigidBody();
            _body.OnCollisionEnter = OnCollisionEnter;
            _body.Position = Physics.PixelsToMeters(LocalToScene(Vector2.Zero));
            _body.LinearVelocity = Physics.PixelsToMeters(_linearVelocity);
            _body.LinearDamping = _linearDamping;
            _body.IsBullet = IsBullet;
            _body.IsSensor = IsSensor;
            _body.IsKinematic = IsKinematic;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            _body?.Dispose();
            _body = null;
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            Broadcast(CollisionEnterEvent, collision);
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            if (IsKinematic)
                _body.Position = Physics.PixelsToMeters(LocalToScene(Vector2.Zero));
            else
            {
                Position = Physics.MetersToPixels(_body.Position);
                _linearVelocity = Physics.MetersToPixels(_body.LinearVelocity);
            }
        }
    }
}
