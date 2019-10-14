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

using System.Runtime.InteropServices;

namespace NoZ
{
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public struct Vector4
    {
        public static readonly Vector4 Zero = new Vector4();
        public static readonly Vector4 One = new Vector4(1f);
        public static readonly Vector4 Half = new Vector4(0.5f);
        public static readonly int SizeInBytes = Marshal.SizeOf(typeof(Vector2));

        public float x;
        public float y;
        public float z;
        public float w;

        public Vector4(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public Vector4(Vector2 xy)
        {
            x = xy.x;
            y = xy.y;
            w = z = 0;
        }

        public Vector4(float v)
        {
            w = x = y = z = v;
        }

        public static bool operator ==(Vector4 lhs, Vector4 rhs) =>
            lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;

        public static bool operator !=(Vector4 lhs, Vector4 rhs) => 
            !(lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z);

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;

            var lhs = this;
            var rhs = (Vector4)obj;
            return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
        }

        public static Vector4 operator +(Vector4 lhs, Vector4 rhs) => new Vector4(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z, lhs.w + rhs.w);
        public static Vector4 operator *(Vector4 lhs, Vector4 rhs) => new Vector4(lhs.x * rhs.x, lhs.y * rhs.y, lhs.z * rhs.z, lhs.w * rhs.w);
        public static Vector4 operator -(Vector4 lhs, Vector4 rhs) => new Vector4(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z, lhs.w - rhs.w);
        public static Vector4 operator /(Vector4 lhs, Vector4 rhs) => new Vector4(lhs.x / rhs.x, lhs.y / rhs.y, lhs.z / rhs.z, lhs.w / rhs.w);
        public static Vector4 operator -(Vector4 lhs) => new Vector4(-lhs.x, -lhs.y, -lhs.z, -lhs.w);
        public static Vector4 operator /(Vector4 lhs, float rhs) => new Vector4(lhs.x / rhs, lhs.y / rhs, lhs.z / rhs, lhs.w / rhs);
        public static Vector4 operator *(Vector4 lhs, float rhs) => new Vector4(lhs.x * rhs, lhs.y * rhs, lhs.z * rhs, lhs.w * rhs);
        public static Vector4 operator +(Vector4 lhs, float rhs) => new Vector4(lhs.x + rhs, lhs.y + rhs, lhs.z + rhs, lhs.w + rhs);
        public static Vector4 operator -(Vector4 lhs, float rhs) => new Vector4(lhs.x - rhs, lhs.y - rhs, lhs.z - rhs, lhs.w - rhs);

        public Vector2 ToVector2() => new Vector2(x, y);
    }
}
