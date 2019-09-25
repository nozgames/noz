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
    public struct Vector2Int
    {
        public static readonly Vector2Int Zero = new Vector2Int();

        public int x;
        public int y;

        public Vector2Int(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector2Int(int v)
        {
            this.x = v;
            this.y = v;
        }

        public int this[int index] {
            get => index == 0 ? x : y;
            set {
                if (index == 0)
                    x = value;
                else
                    y = value;
            }
        }

        public static bool operator ==(Vector2Int lhs, Vector2Int rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y;
        }

        public static bool operator !=(Vector2Int lhs, Vector2Int rhs)
        {
            return !(lhs.x == rhs.x && lhs.y == rhs.y);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;

            Vector2Int lhs = this;
            Vector2Int rhs = (Vector2Int)obj;
            return lhs.x == rhs.x && lhs.y == rhs.y;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode();
        }

        public Vector2 ToVector2() => new Vector2(x, y);
        public Vector2Double ToVector2Double() => new Vector2Double(x, y);


        public static Vector2Int Parse(string value)
        {
            string[] parts = value.Split(',');
            if (null == parts || parts.Length == 0)
                return Zero;

            if (parts.Length == 1)
            {
                int.TryParse(parts[0], out var parsed);
                return new Vector2Int(parsed);
            }

            int.TryParse(parts[0], out var parsedX);
            int.TryParse(parts[1], out var parsedY);
            return new Vector2Int(parsedX, parsedY);
        }

        public override string ToString()
        {
            if (x == y)
                return $"{x}";

            return $"{x},{y}";
        }

        public static Vector2Int operator +(Vector2Int lhs, Vector2Int rhs)
        {
            return new Vector2Int(lhs.x + rhs.x, lhs.y + rhs.y);
        }

        public static Vector2Int operator *(Vector2Int lhs, Vector2Int rhs)
        {
            return new Vector2Int(lhs.x * rhs.x, lhs.y * rhs.y);
        }

        public static Vector2Int operator -(Vector2Int lhs, Vector2Int rhs)
        {
            return new Vector2Int(lhs.x - rhs.x, lhs.y - rhs.y);
        }

        public static Vector2Int operator /(Vector2Int lhs, Vector2Int rhs)
        {
            return new Vector2Int(lhs.x / rhs.x, lhs.y / rhs.y);
        }

        public static Vector2Int operator /(Vector2Int lhs, int rhs) => new Vector2Int(lhs.x / rhs, lhs.y / rhs);
        public static Vector2Int operator *(Vector2Int lhs, int rhs) => new Vector2Int(lhs.x * rhs, lhs.y * rhs);
        public static Vector2Int operator +(Vector2Int lhs, int rhs) => new Vector2Int(lhs.x + rhs, lhs.y + rhs);
        public static Vector2Int operator -(Vector2Int lhs, int rhs) => new Vector2Int(lhs.x - rhs, lhs.y - rhs);


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
