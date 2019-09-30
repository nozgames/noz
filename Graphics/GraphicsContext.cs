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
        public struct State
        {
            public int maskCount;
            public Color color;
            public float alpha;
        }

        private Stack<State> _state;
        private Stack<Matrix3> _worldToScreen;

        public static GraphicsContext Create() => Graphics.Driver.CreateContext();

        protected GraphicsContext()
        {
            _state = new Stack<State>(4);
            _worldToScreen = new Stack<Matrix3>(4);
            _worldToScreen.Push(Matrix3.Identity);
        }

        public virtual void Begin(Vector2Int size, Color backgroundColor)
        {
            _state.Clear();
        }

        public virtual void End()
        {
        }

        private State CurrentState {
            get {
                return _state.Peek();
            }
        }

        public void PushState()
        {
            if (_state.Count > 0)
            {
                _state.Push(new State()
                {
                    color = CurrentState.color,
                    alpha = CurrentState.alpha
                });
            }
            else
            {
                _state.Push(new State()
                {
                    color = Color.White,
                    alpha = 1f
                });
            }
        }

        public void PopState()
        {
            State state = CurrentState;

            // Pop any outstanding masks
            while (state.maskCount > 0)
                throw new NotImplementedException();

            _state.Pop();
        }

        /// <summary>
        /// Save the current mask and push a new one on the stack.
        /// </summary>
        public abstract void PushMask();

        /// <summary>
        /// Remove the current mask and revert back to the previous mask
        /// </summary>
        public abstract void PopMask();

        public abstract void SetMaskMode(MaskMode mode);

        public abstract void SetColor(Color color);

        public abstract void SetTransform(in Matrix3 transform);

        public abstract void SetImage(in Image image);

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
    }
}

