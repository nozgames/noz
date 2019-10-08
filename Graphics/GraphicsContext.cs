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
    public enum PrimitiveType
    {
        TriangleList,
        TriangleStrip,
        LineList,
        LineStrip
    }

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
        public class State
        {
            public int maskCount;
            public Color color;
            public float opacity;
        }

        private static ObjectPool<State> _statePool = new ObjectPool<State>(() => new State(), 16);
        private Stack<State> _state;
        private Stack<Matrix3> _worldToScreen;
        private Color _color;

        public static GraphicsContext Create() => Graphics.Driver.CreateContext();

        protected GraphicsContext()
        {
            _state = new Stack<State>(4);
            _worldToScreen = new Stack<Matrix3>(4);
            _worldToScreen.Push(Matrix3.Identity);

            _state.Push(new State { color = Color.White, opacity = 1.0f, maskCount = 0 });
        }

        public virtual void Begin(Vector2Int size, Color backgroundColor)
        {
            _color = Color.White;
        }

        public virtual void End()
        {
            // Pop any states remaining
            while (_state.Count > 1) _state.Pop();
        }

        /// <summary>
        /// Get the current state
        /// </summary>
        private State CurrentState => _state.Peek();

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

            _color = CurrentState.color.MultiplyAlpha(CurrentState.opacity);
        }

        /// <summary>
        /// Save the current mask and push a new one on the stack.
        /// </summary>
        public abstract void PushMask();

        /// <summary>
        /// Remove the current mask and revert back to the previous mask
        /// </summary>
        public abstract void PopMask();

        public abstract MaskMode MaskMode { get; set; }

        public Color Color {
            get => CurrentState.color;
            set {
                CurrentState.color = value;
                UpdateColor();
            }
        }

        public float Opacity {
            get => CurrentState.opacity;
            set {
                CurrentState.opacity = value;
                UpdateColor();
            }
        }

        public Color ColorWithOpacity => _color;

        public abstract Image Image { get; set; }

        public abstract Matrix3 Transform { get; set; }

        public abstract void Draw(PrimitiveType primitive, Vertex[] vertexBuffer, int vertexCount, short[] indexBuffer, int indexCount);

        public abstract void Draw(PrimitiveType primitive, Vertex[] vertexBuffer, int vertexCount);

        public abstract void Draw(in Quad quad);

        public abstract void Draw(Quad[] quad, int count);

        public void PushMatrix(in Matrix3 worldToScreen)
        {
            _worldToScreen.Push(worldToScreen);
        }

        public void PopMatrix()
        {
            if (_worldToScreen.Count > 1)
                _worldToScreen.Pop();
        }

        public Matrix3 WorldToScreen => _worldToScreen.Peek();

        private void UpdateColor () => _color = CurrentState.color.MultiplyAlpha(CurrentState.opacity);
    }
}

