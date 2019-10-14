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

namespace NoZ
{
    public struct QueryResults
    {
        private ICollider[] _results;
        private int _length;

        public int Length => _length;

        public ICollider this [int index] => _results[index];

        public QueryResults (ICollider[] results, int length)
        {
            _results = results;
            _length = length;
        }
    }

    public class World : Object, IDisposable
    {
        public static readonly Event<float> UpdateEvent = new Event<float>();
        
        private IWorld _world;
        private Rect _bounds;
        private IBody _boundsBody;
        private ICollider _boundsCollider;
        private ICollider[] _queryResults;

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

        public QueryResults Query(in Rect rect, uint mask, int count)
        {
            if (_queryResults == null || _queryResults.Length < count)
                _queryResults = new ICollider[count];

            return new QueryResults(
                _queryResults, 
                _world.Query(
                    Physics.PixelsToMeters(rect),
                    mask,
                    _queryResults,
                    0,
                    count
                )
            );
        }

        public void Step()
        {
            UpdateEvent.Broadcast(this, Time.FixedDeltaTime);
            _world.Step(Time.FixedDeltaTime);
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

            _boundsBody?.Dispose();
            _boundsBody = null;

            if (_bounds == Rect.Empty)
                return;

            _boundsBody = CreateStaticBody();
            _boundsBody.AddEdgeCollider(Physics.PixelsToMeters(Bounds.TopLeft), Physics.PixelsToMeters(Bounds.BottomLeft));
            _boundsBody.AddEdgeCollider(Physics.PixelsToMeters(Bounds.TopLeft), Physics.PixelsToMeters(Bounds.TopRight));
            _boundsBody.AddEdgeCollider(Physics.PixelsToMeters(Bounds.TopRight), Physics.PixelsToMeters(Bounds.BottomRight));
            _boundsBody.AddEdgeCollider(Physics.PixelsToMeters(Bounds.BottomRight), Physics.PixelsToMeters(Bounds.BottomLeft));
            _boundsBody.CollidesWithMask = Physics.CollisionMaskAll;
            _boundsBody.CollisionMask = Physics.CollisionMaskAll;
        }
    }
}

