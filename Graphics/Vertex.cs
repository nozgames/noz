using System;
using System.Runtime.InteropServices;

namespace NoZ.Graphics {

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public struct Vertex {
        public static readonly int SizeInBytes = Marshal.SizeOf<Vertex>();
        public static readonly int OffsetUV = (int)Marshal.OffsetOf<Vertex>("UV");
        public static readonly int OffsetColor = (int)Marshal.OffsetOf<Vertex>("Color");
        public static readonly int OffsetXY = (int)Marshal.OffsetOf<Vertex>("XY");

        public Vector2 XY;
        public Color Color;
        public Vector2 UV;

        public Vertex (in Vector2 xy, in Vector2 uv, Color color) {
            XY = xy;
            UV = uv;
            Color = color;
        }

        public Vertex (in Vector2 xy, in Vector2 uv) {
            XY = xy;
            UV = uv;
            Color = Color.White;
        }

        public Vertex(in Vector2 xy) {
            XY = xy;
            UV = Vector2.Zero;
            Color = Color.White;
        }

        public Vertex(float x, float y) {
            XY = new Vector2(x,y);
            UV = Vector2.Zero;
            Color = Color.White;
        }

        public Vertex(float x, float y, Color color) {
            XY = new Vector2(x, y);
            UV = Vector2.Zero;
            Color = color;
        }
    }

}
