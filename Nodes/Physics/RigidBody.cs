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
    public class RigidBody : PhysicsNode
    {
        internal IBody _body;
        private Vector2 _linearVelocity;        

        public bool IsKinematic { get; set; }

        public Vector2 LinearVelocity {
            get => _linearVelocity;
            set {
                _linearVelocity = value;
                if (_body != null)
                    _body.LinearVelocity = _linearVelocity;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _body = Scene.World.CreateRigidBody();
            _body.OnCollisionEnter = OnCollisionEnter;
            _body.Position = LocalToWorld.MultiplyVector(Vector2.Zero);
            _body.LinearVelocity = _linearVelocity;
            _body.Layers = Layers;
            _body.CollidesWithLayers = CollidesWithLayers;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            _body?.Dispose();
            _body = null;
        }

        protected virtual bool OnCollisionEnter(Collision collision)
        {
            Broadcast(CollisionEnterEvent, collision);
            return true;
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            if(IsKinematic)
                _body.Position = LocalToWorld.MultiplyVector(Vector2.Zero);
            else
                Position = _body.Position;
        }
    }
}
