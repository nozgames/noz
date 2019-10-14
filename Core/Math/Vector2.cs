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
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;

namespace NoZ
{
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    [TypeConverter(typeof(Vector2TypeConverter))]
    public struct Vector2
    {
        public static readonly Vector2 Zero = new Vector2();
        public static readonly Vector2 One = new Vector2(1f);
        public static readonly Vector2 Half = new Vector2(0.5f);
        public static readonly Vector2 OneZero = new Vector2(1f, 0f);
        public static readonly Vector2 ZeroOne = new Vector2(0f, 1f);
        public static readonly Vector2 NaN = new Vector2(float.NaN, float.NaN);
        public static readonly Vector2 Infinity = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
        public static readonly int SizeInBytes = Marshal.SizeOf(typeof(Vector2));

        public float x;
        public float y;

        public float this[int index] {
            get {
                return index == 0 ? x : y;
            }
            set {
                if (index == 0)
                    x = value;
                else
                    y = value;
            }
        }

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector2(float v)
        {
            x = y = v;
        }

        /// Returns the minimum component of the vector
        public float Min() => MathEx.Min(x, y);

        /// Returns the maximum component of the vector
        public float Max() => MathEx.Max(x, y);

        public static Vector2 Mix(Vector2 a, Vector2 b, float weight) =>
            new Vector2(MathEx.Mix(a.x, b.x, weight), MathEx.Mix(a.y, b.y, weight));

        public static float Dot(Vector2 lhs, Vector2 rhs) => lhs.x * rhs.x + lhs.y * rhs.y;

        public static float Cross(Vector2 lhs, Vector2 rhs) => (lhs.x * rhs.y) - (lhs.y * rhs.x);

        public float Magnitude => MathEx.Sqrt(x * x + y * y);

        public float MagnitudeSquared => x * x + y * y;

        public Vector2 Normalized {
            get {
                float l = Magnitude;
                return new Vector2(x / l, y / l);
            }
        }

        public Vector2 OrthoNormalized {
            get {
                var len = Magnitude;
                return new Vector2(-y / len, x / len);
            }
        }

        public Vector2 NormalizedSafe {
            get {
                float l = Magnitude;

                // Prevent divide by zero crashes
                if (l == 0)
                    return ZeroOne;

                return new Vector2(x / l, y / l);
            }
        }

        /// Return a vector that contains the maxium values of both components
        public static Vector2 Max(in Vector2 a, in Vector2 b)
        {
            return new Vector2(MathEx.Max(a.x, b.x), MathEx.Max(a.y, b.y));
        }

        /// Return a vector that contains the minimum values of both components
        public static Vector2 Min(in Vector2 a, in Vector2 b)
        {
            return new Vector2(MathEx.Min(a.x, b.x), MathEx.Min(a.y, b.y));
        }

        public static Vector2 Clamp(in Vector2 v, in Vector2 min, in Vector2 max)
        {
            return new Vector2(MathEx.Clamp(v.x, min.x, max.x), MathEx.Clamp(v.y, min.y, max.y));
        }

        /// <summary>
        /// Return vector perpendicular to the given vector
        /// </summary>
        public static Vector2 Perpendicular(in Vector2 v) => new Vector2(-v.y, v.x);

        public static Vector2 operator +(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2(lhs.x + rhs.x, lhs.y + rhs.y);
        }

        public static Vector2 operator *(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2(lhs.x * rhs.x, lhs.y * rhs.y);
        }

        public static Vector2 operator -(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2(lhs.x - rhs.x, lhs.y - rhs.y);
        }

        public static Vector2 operator /(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2(lhs.x / rhs.x, lhs.y / rhs.y);
        }

        public static Vector2 operator -(Vector2 lhs)
        {
            return new Vector2(-lhs.x, -lhs.y);
        }

        public static Vector2 operator /(Vector2 lhs, float rhs) => new Vector2(lhs.x / rhs, lhs.y / rhs);
        public static Vector2 operator *(Vector2 lhs, float rhs) => new Vector2(lhs.x * rhs, lhs.y * rhs);
        public static Vector2 operator +(Vector2 lhs, float rhs) => new Vector2(lhs.x + rhs, lhs.y + rhs);
        public static Vector2 operator -(Vector2 lhs, float rhs) => new Vector2(lhs.x - rhs, lhs.y - rhs);

        public static bool operator ==(Vector2 lhs, Vector2 rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y;
        }

        public static bool operator !=(Vector2 lhs, Vector2 rhs)
        {
            return !(lhs.x == rhs.x && lhs.y == rhs.y);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;

            Vector2 lhs = this;
            Vector2 rhs = (Vector2)obj;
            return lhs.x == rhs.x && lhs.y == rhs.y;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode();
        }

        public static Vector2 Parse(string value)
        {
            string[] parts = value.Split(',');
            if (null == parts || parts.Length == 0)
                return Zero;

            if (parts.Length == 1)
            {
                float.TryParse(parts[0], out var parsed);
                return new Vector2(parsed);
            }

            float.TryParse(parts[0], out var parsedX);
            float.TryParse(parts[1], out var parsedY);
            return new Vector2(parsedX, parsedY);
        }

        public override string ToString()
        {
            if (x == y)
                return $"{x}";

            return $"{x},{y}";
        }

        public Vector3 ToVector3() => new Vector3(x, y, 0);

        public Vector2Int ToVector2Int() => new Vector2Int((int)x, (int)y);

        public float DistanceToLineSegmentSquared(in Vector2 pt1, in Vector2 pt2)
        {
            var seg = (pt2 - pt1);
            var l2 = seg.MagnitudeSquared;
            var t = ((x - pt1.x) * seg.x + (y - pt1.y) * seg.y) / l2;
            if (t < 0f) return (this - pt1).MagnitudeSquared;
            if (t > 1f) return (this - pt2).MagnitudeSquared;
            return new Vector2(pt1.x + t * seg.x, pt1.y + t * seg.y).MagnitudeSquared;
        }

        private class Vector2TypeConverter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                if (sourceType == typeof(string)) return true;
                return base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                if (value == null) return Zero;
                if (value is string) return Parse((string)value);
                return base.ConvertFrom(context, culture, value);
            }
        }
    }
}
