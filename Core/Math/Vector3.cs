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
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NoZ {
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public struct Vector3 {
        public static readonly Vector3 Zero = new Vector3();
        public static readonly Vector3 One = new Vector3(1f);
        public static readonly Vector3 Half = new Vector3(0.5f);
        public static readonly int SizeInBytes = Marshal.SizeOf(typeof(Vector2));

        public float x;
        public float y;
        public float z;

        public Vector3(float x, float y, float z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3(Vector2 xy) {
            x = xy.x;
            y = xy.y;
            z = 0;
        }

        public Vector3(float v) {
            x = y = z = v;
        }

        public static bool operator ==(Vector3 lhs, Vector3 rhs) {
            return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
        }

        public static bool operator !=(Vector3 lhs, Vector3 rhs) {
            return !(lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z);
        }

        public override bool Equals(object obj) {
            if (obj == null || obj.GetType() != GetType())
                return false;

            Vector3 lhs = this;
            Vector3 rhs = (Vector3)obj;
            return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
        }

        public override int GetHashCode() {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
        }

        public static Vector3 operator +(Vector3 lhs, Vector3 rhs) => new Vector3(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);
        public static Vector3 operator *(Vector3 lhs, Vector3 rhs) => new Vector3(lhs.x * rhs.x, lhs.y * rhs.y, lhs.z * rhs.z);
        public static Vector3 operator -(Vector3 lhs, Vector3 rhs) => new Vector3(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z);
        public static Vector3 operator /(Vector3 lhs, Vector3 rhs) => new Vector3(lhs.x / rhs.x, lhs.y / rhs.y, lhs.z / rhs.z);
        public static Vector3 operator -(Vector3 lhs) => new Vector3(-lhs.x, -lhs.y, -lhs.z);
        public static Vector3 operator /(Vector3 lhs, float rhs) => new Vector3(lhs.x / rhs, lhs.y / rhs, lhs.z / rhs);
        public static Vector3 operator *(Vector3 lhs, float rhs) => new Vector3(lhs.x * rhs, lhs.y * rhs, lhs.z * rhs);
        public static Vector3 operator +(Vector3 lhs, float rhs) => new Vector3(lhs.x + rhs, lhs.y + rhs, lhs.z + rhs);
        public static Vector3 operator -(Vector3 lhs, float rhs) => new Vector3(lhs.x - rhs, lhs.y - rhs, lhs.z - rhs);

        public Vector2 ToVector2() => new Vector2(x, y);
    }
}
