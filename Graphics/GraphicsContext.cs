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
using System.Collections.Generic;

namespace NoZ
{
    public enum MaskMode
    {
        /// <summary>
        /// Indicates that masks should be ignored.
        /// </summary>
        None,

        /// <summary>
        /// Indicates that all draw operations should write to the mask instead of the color buffer.
        /// </summary>
        Draw,

        /// <summary>
        /// Indicates that all pixels rendered inside of the current mask should be drawn
        /// </summary>
        Inside,

        /// <summary>
        /// Indicates that all fragments rendered outside of the current mask should be drawn
        /// </summary>
        Outside
    }

    public abstract class GraphicsContext
    {
        private const float DefaultTransparencySortIncrement = 0.001f;

        public class State
        {
            public int maskCount;
            public Color color;
            public float opacity;
        }

        /// <summary>
        /// Pool to reuse states
        /// </summary>
        private static ObjectPool<State> _statePool = new ObjectPool<State>(() => new State(), 16);

        private Stack<State> _state;
        private Stack<Matrix3> _transform;
        private Stack<ushort> _sortGroupStack = new Stack<ushort>();

        private List<DrawNode> _nodes = new List<DrawNode>();
        private List<DrawNode> _sortGroups = new List<DrawNode>();
        private int _nodeCount = 0;


        public byte SortLayer { get; set; }

        public short SortOrder { get; set; }

        public static GraphicsContext Create() => Graphics.Driver.CreateContext();

        protected GraphicsContext()
        {
            _transform = new Stack<Matrix3>(16);
            _transform.Push(Matrix3.Identity);
            _state = new Stack<State>(16);
            _state.Push(new State { color = Color.White, opacity = 1.0f, maskCount = 0 });
            _nodes.Add(new DrawNode());
        }

        protected internal virtual void Begin (Vector2Int size, Color backgroundColor) { }

        protected internal virtual void End () { }

        private void DrawGroup(DrawNode group)
        {
            for (int i = 0; i < group.GroupCount; i++)
            {
                var dnode = _nodes[group.GroupStart + i];
                switch (dnode.Type)
                {
                    case DrawNodeType.Group:
                        DrawGroup(dnode);
                        break;

                    case DrawNodeType.DebugLine:
                    case DrawNodeType.Quad:
                        Draw(dnode);
                        break;
                }
            }
        }

        public void BatchBegin()
        {
            TransparencySortMode = TransparencySortMode.Default;

            _nodeCount = 1;
            _sortGroupStack.Clear();
            _sortGroups.Clear();

            var dnode = _nodes[0];
            dnode.Type = DrawNodeType.Group;
            dnode.Index = (ushort)(_nodeCount);
            dnode.color = Color.White;
            dnode.SortLayer = 0;
            dnode.SortOrder = 0;
            dnode.SortGroup = 0;
            dnode.TransparencySort = 0.0f;
            dnode.GroupCount = 0;
            dnode.GroupStart = 0;

            _sortGroupStack.Push(1);
            _sortGroups.Add(dnode);
        }

        public void BatchEnd()
        {
            if (_nodeCount == 0)
                return;

            PopSortGroup();

            // Sort the nodes 
            _nodes.Sort(0, _nodeCount, DrawNode.Comparer.Instance);

            // Determine the start node of all groups
            for (int groupIndex = 0, nodeIndex = 1; groupIndex < _sortGroups.Count; nodeIndex += _sortGroups[groupIndex].GroupCount, groupIndex++)
                _sortGroups[groupIndex].GroupStart = (ushort)nodeIndex;

            // Render each node
            DrawGroup(_nodes[0]);

            // Pop any states remaining
            while (_state.Count > 1) PopState();
        }

        /// <summary>
        /// Get the current state
        /// </summary>
        private State CurrentState => _state.Peek();

        /// <summary>
        /// Current mask mode
        /// </summary>
        public MaskMode MaskMode { get; set; }

        /// <summary>
        /// Current color
        /// </summary>
        public Color Color { get; set; }

        public float Opacity {
            get => CurrentState.opacity;
            set => CurrentState.opacity = value;
        }

        public TransparencySortMode TransparencySortMode { get; set; } = TransparencySortMode.Default;

        /// <summary>
        /// Image used to render any subsequent Draw calls
        /// </summary>
        public Image Image { get; set; }

        /// <summary>
        /// Push the current state onto the stack
        /// </summary>
        public void PushState()
        {
            var state = _statePool.Get();
            state.color = CurrentState.color;
            state.opacity = CurrentState.opacity;
            state.maskCount = 0;
            _state.Push(state);
        }

        /// <summary>
        /// Pop the current state from the stack
        /// </summary>
        public void PopState()
        {
            if (_state.Count < 1)
                return;

            // Pop any outstanding masks
            var current = CurrentState;
            while (current.maskCount > 0)
                throw new NotImplementedException();

            _state.Pop();

            _statePool.Release(current);
        }

        /// <summary>
        /// Save the current mask and push a new one on the stack.
        /// </summary>
        public void PushMask() => throw new NotImplementedException();

        /// <summary>
        /// Remove the current mask and revert back to the previous mask
        /// </summary>
        public void PopMask() => throw new NotImplementedException();

        /// <summary>
        /// Push the current transform on to the transform stack
        /// </summary>
        public void PushTransform () => _transform.Push(Transform);

        /// <summary>
        /// Push the current transform multiplied by the given transform to the transform stack
        /// </summary>
        public void PushMultipliedTransform(in Matrix3 mat) => _transform.Push(Matrix3.Multiply(mat, _transform.Peek()));

        /// <summary>
        /// Push the given transform onto the transform stack
        /// </summary>
        public void PushTransform (in Matrix3 mat) => _transform.Push(mat);

        /// <summary>
        /// Pop the current transform off of the transform stack
        /// </summary>
        public void PopTransform ()
        {
            if (_transform.Count > 1)
                _transform.Pop();
        }

        /// <summary>
        /// Multiply the current transform by the given transform
        /// </summary>
        /// <param name="mat"></param>
        public void MultiplyTransform (in Matrix3 mat) => _transform.Push(Matrix3.Multiply(mat, _transform.Pop()));

        /// <summary>
        /// Current transform on the transform stack
        /// </summary>
        public Matrix3 Transform {
            get => _transform.Peek();
            set {
                _transform.Pop();
                _transform.Push(value);
            }
        }

        public void PushSortGroup ()
        {
            var index = _nodeCount++;
            if (index >= _nodes.Count)
                _nodes.Add(new DrawNode());

            var dnode = _nodes[index];

            switch (TransparencySortMode)
            {
                case TransparencySortMode.Default:
                    dnode.TransparencySort = index * DefaultTransparencySortIncrement;
                    break;

                case TransparencySortMode.YAxis:
                    dnode.TransparencySort = Transform.MultiplyVector(Vector2.Zero).y;
                    break;
            }

            dnode.Type = DrawNodeType.Group;
            dnode.Index = (ushort)index;
            dnode.color = Color.White;
            dnode.SortLayer = SortLayer;
            dnode.SortOrder = SortOrder;
            dnode.SortGroup = _sortGroupStack.Peek();
            dnode.image = null;
            dnode.GroupCount = 0;
            dnode.GroupStart = 0;

            // Maintain the node count in the group
            _sortGroups[dnode.SortGroup - 1].GroupCount++;

            _sortGroups.Add(dnode);
            _sortGroupStack.Push((ushort)_sortGroups.Count);
        }

        public void PopSortGroup()
        {
            _sortGroupStack.Pop();
        }

        /// <summary>
        /// Draw a single quad
        /// </summary>
        public void Draw(in Quad quad)
        {
            var tquad = quad;
            tquad.TL.XY = Transform.MultiplyVector(quad.TL.XY);
            tquad.TR.XY = Transform.MultiplyVector(quad.TR.XY);
            tquad.BR.XY = Transform.MultiplyVector(quad.BR.XY);
            tquad.BL.XY = Transform.MultiplyVector(quad.BL.XY);

            var index = _nodeCount++;
            if (index >= _nodes.Count)
                _nodes.Add(new DrawNode());

            var transparencySort = 0.0f;
            switch (TransparencySortMode)
            {
                case TransparencySortMode.Default: 
                    transparencySort = index * DefaultTransparencySortIncrement; 
                    break;

                case TransparencySortMode.YAxis: 
                    transparencySort = (tquad.TL.XY.y + tquad.TR.XY.y + tquad.BL.XY.y + tquad.BR.XY.y) * 0.25f;
                    break;
            }

            var dnode = _nodes[index];
            dnode.Type = DrawNodeType.Quad;
            dnode.quad = tquad;
            dnode.Index = (ushort)index;
            dnode.color = Color.MultiplyAlpha(Opacity);
            dnode.SortLayer = SortLayer;
            dnode.SortOrder = SortOrder;
            dnode.SortGroup = _sortGroupStack.Peek();
            dnode.TransparencySort = transparencySort;
            dnode.image = Image;

            // Maintain the node count in the group
            _sortGroups[dnode.SortGroup - 1].GroupCount++;
        }

        /// <summary>
        /// Draw a batch of quads
        /// </summary>
        public void Draw(Quad[] quad, int offset, int count)
        {
            for (int i = 0; i < count; i++)
                Draw(quad[i + offset]);
        }

        /// <summary>
        /// Override to draw a node
        /// </summary>
        protected abstract void Draw(DrawNode node);


        /// <summary>
        /// Draw a debug line on top 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void DrawDebugLine (in Vector2 from, in Vector2 to)
        {
            var index = _nodeCount++;
            if (index >= _nodes.Count)
                _nodes.Add(new DrawNode());

            var dnode = _nodes[index];
            dnode.Type = DrawNodeType.DebugLine;
            dnode.quad = new Quad { 
                TL = new Vertex(Transform.MultiplyVector(from)),
                TR = new Vertex(Transform.MultiplyVector(to)) };
            dnode.Index = (ushort)index;
            dnode.color = Color.MultiplyAlpha(Opacity);
            dnode.SortLayer = 255;
            dnode.SortOrder = 0;
            dnode.SortGroup = 1;
            dnode.TransparencySort = 0.0f;
            dnode.image = null;

            _sortGroups[0].GroupCount++;
        }
    }
}

