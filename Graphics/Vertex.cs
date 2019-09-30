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
using System.Runtime.InteropServices;

namespace NoZ
{
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public struct Vertex
    {
        public static readonly int SizeInBytes = Marshal.SizeOf<Vertex>();
        public static readonly int OffsetUV = (int)Marshal.OffsetOf<Vertex>("UV");
        public static readonly int OffsetColor = (int)Marshal.OffsetOf<Vertex>("Color");
        public static readonly int OffsetXY = (int)Marshal.OffsetOf<Vertex>("XY");

        public Vector2 XY;
        public Color Color;
        public Vector2 UV;

        public Vertex(in Vector2 xy, in Vector2 uv, Color color)
        {
            XY = xy;
            UV = uv;
            Color = color;
        }

        public Vertex(in Vector2 xy, in Vector2 uv)
        {
            XY = xy;
            UV = uv;
            Color = Color.White;
        }

        public Vertex(in Vector2 xy)
        {
            XY = xy;
            UV = Vector2.Zero;
            Color = Color.White;
        }

        public Vertex(float x, float y)
        {
            XY = new Vector2(x, y);
            UV = Vector2.Zero;
            Color = Color.White;
        }

        public Vertex(float x, float y, Color color)
        {
            XY = new Vector2(x, y);
            UV = Vector2.Zero;
            Color = color;
        }
    }

}
