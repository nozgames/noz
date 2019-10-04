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

using System;

namespace NoZ.Physics
{
    public class World : Object, IDisposable
    {
        public static readonly Event<float> UpdateEvent = new Event<float>();
        
        private IWorld _world;
        private float _accumulatedTime;
        private Rect _bounds;
        private ICollider _boundsCollider;

        public uint DebugVisualizationMask { get; set; } 

        public Rect Bounds {
            get => _bounds;
            set {
                if (_bounds == value)
                    return;

                _bounds = value;

                UpdateBoundsCollider();
            }
        }

        public World ()
        {
            _world = Physics.CreateWorld();
        }

        public IBody CreateRigidBody() => _world.CreateRigidBody();
        public IBody CreateKinematicBody() => _world.CreateKinematicBody();
        public IBody CreateStaticBody() => _world.CreateStaticBody();

        public void Step()
        {
            _accumulatedTime += Time.DeltaTime;
            while(_accumulatedTime > Time.FixedUnscaledDeltaTime)
            {
                Time.BeginFixedTimeStep();
                UpdateEvent.Broadcast(this, Time.FixedDeltaTime);
                _world.Step(Time.FixedDeltaTime);
                Time.EndFixedTimeStep();
                _accumulatedTime -= Time.FixedUnscaledDeltaTime;
            }
        }

        public void DrawDebug(GraphicsContext gc)
        {
            if(DebugVisualizationMask != Physics.CollisionMaskNone)
                _world.DrawDebug(gc, DebugVisualizationMask);
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        private void UpdateBoundsCollider()
        {
            _boundsCollider?.Dispose();
            _boundsCollider = null;

            if (_bounds == Rect.Empty)
                return;

            // tODO: CREATE bounds collider with edge, collide with all
        }
    }
}

