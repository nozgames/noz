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

namespace NoZ {

    public enum ColorMode {
        Override,
        Inherit,
        Multiply
    }

    [TypeConverter(typeof(ColorTypeConverter))]
    public struct Color {
        public static readonly Color Empty = new Color();
        public static readonly Color White = FromRgb(255, 255, 255);
        public static readonly Color Black = FromRgb(0, 0, 0);
        public static readonly Color Blue = FromRgb(0, 0, 255);
        public static readonly Color Red = FromRgb(255, 0, 0);
        public static readonly Color Green = FromRgb(0, 255, 0);
        public static readonly Color Transparent = FromRgba(0, 0, 0, 0);

        private static float ByteToFloat(byte b) => b * (1.0f / 255.0f);
        private static byte FloatToByte(float f) => (byte)(f * 255.0f);

        private const int RedShift = 0;
        private const int GreenShift = 8;
        private const int BlueShift = 16;
        private const int AlphaShift = 24;

        private Color(uint value) {
            Value = value;
        }

        public uint Value {
            get; private set;
        }

        public byte R {
            get {
                return (byte)((Value >> RedShift) & 0xFF);
            }
        }

        public byte G {
            get {
                return (byte)((Value >> GreenShift) & 0xFF);
            }
        }

        public byte B {
            get {
                return (byte)((Value >> BlueShift) & 0xFF);
            }
        }

        public byte A {
            get {
                return (byte)((Value >> AlphaShift) & 0xFF);
            }
        }

        public static Color FromRgba(in Vector4 rgba) => FromRgba(rgba.x, rgba.y, rgba.z, rgba.w);

        public static Color FromRgb(in Vector3 rgb) => FromRgba(rgb.x, rgb.y, rgb.z, 1.0f);
        public static Color FromRgba(in Vector3 rgb, float a) => FromRgba(rgb.x, rgb.y, rgb.z, a);

        public static Color FromRgba(float r, float g, float b, float a) =>
            FromRgba(FloatToByte(r), FloatToByte(g), FloatToByte(b), FloatToByte(a));

        public static Color FromRgb(byte r, byte g, byte b) => FromRgba(r, g, b, 255);

        public static Color FromRgba(byte r, byte g, byte b, byte a) => new Color(MakeRgba(r, g, b, a));

        public static Color FromRgba(Color from, byte a) => FromRgba(from.R, from.G, from.B, a);
        public static Color FromRgba(Color from, float a) => FromRgba(from.R, from.G, from.B, FloatToByte(a));

        public static Color FromUInt32 (uint value) => new Color(value);

        private static uint MakeRgba(byte r, byte g, byte b, byte a) {
            return (uint)
                (unchecked((uint)(
                    r << RedShift |
                    g << GreenShift |
                    b << BlueShift |
                    a << AlphaShift)));
        }

        public static Color Parse(string value) {
            if (string.IsNullOrEmpty(value))
                return White;

            if (value[0] == '#')
                value = value.Substring(1);

            if (value.Length == 1)
                value = string.Format("FF{0}{0}{0}{0}{0}{0}", value[0]);
            else if (value.Length == 3)
                value = string.Format("FF{0}{0}{1}{1}{2}{2}", value[2], value[1], value[0]);
            else if (value.Length == 4)
                value = string.Format("{0}{0}{1}{1}{2}{2}{3}{3}", value[3], value[2], value[1], value[0]);
            else if (value.Length == 5)
                value = string.Format("FF0{0}{1}{2}{3}{4}", value[4], value[2], value[3], value[0], value[1]);
            else if (value.Length == 6)
                value = string.Format("FF{0}{1}{2}{3}{4}{5}", value[4], value[5], value[2], value[3], value[0], value[1]);
            else if (value.Length == 7)
                value = string.Format("0{0}{1}{2}{3}{4}{5}{6}", value[6], value[4], value[5], value[2], value[3], value[0], value[1]);
            else if (value.Length >= 8)
                value = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}", value[6], value[7], value[4], value[5], value[2], value[3], value[0], value[1]);

            return new Color(uint.Parse(value, System.Globalization.NumberStyles.HexNumber));
        }

        public override string ToString() {
            if(A==255)
                return $"#{R.ToString("X2")}{G.ToString("X2")}{B.ToString("X2")}";

            return $"#{R.ToString("X2")}{G.ToString("X2")}{B.ToString("X2")}{A.ToString("X2")}";
        }

        public Vector3 ToVector3() => new Vector3(ByteToFloat(R), ByteToFloat(G), ByteToFloat(B));
        public Vector4 ToVector4() => new Vector4(ByteToFloat(R), ByteToFloat(G), ByteToFloat(B), ByteToFloat(A));

        public Color Scale (float value) =>
            FromRgba((byte)(R * value), (byte)(G * value), (byte)(B * value), (byte)(A * value));

        /// <summary>
        /// Multiply the alpha component of the color by the given alpha value and return
        /// a new color.
        /// </summary>
        /// <param name="value">Value to multiply the alpha by.</param>
        /// <returns>
        /// New color that contains the same RGB components of the original and the
        /// alpha value multiplied by the given value.
        /// </returns>
        public Color MultiplyAlpha(byte value) => FromRgba(R, G, B, (byte)(((value * A) / 255) & 0xFF));

        /// <summary>
        /// Multiply the alpha component of the color by the given alpha value and return
        /// a new color.
        /// </summary>
        /// <param name="value">Value to multiply the alpha by.</param>
        /// <returns>
        /// New color that contains the same RGB components of the original and the
        /// alpha value multiplied by the given value.
        /// </returns>
        public Color MultiplyAlpha(float value) => FromRgba(R, G, B, (byte)(value * A));

        public static Color Lerp (Color from, Color to, float lerp) {
            var ilerp = 1.0f - lerp;
            return FromRgba(
              (byte)(ilerp * from.R + (lerp * to.R)),
              (byte)(ilerp * from.G + (lerp * to.G)),
              (byte)(ilerp * from.B + (lerp * to.B)),
              (byte)(ilerp * from.A + (lerp * to.A))
            );
        }

        public static Color LerpAlpha(Color color, float from, float to, float lerp)
        {
            return FromRgba(color.R, color.G, color.B, (byte)((1.0f - lerp) * from + lerp * to * 255.0f));
        }

        private class ColorTypeConverter : TypeConverter {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
                if (sourceType == typeof(string))
                    return true;
                return base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
                if (value == null)
                    return White;
                if (value is string)
                    return Color.Parse((string)value);
                return base.ConvertFrom(context, culture, value);
            }
        }

        public static Color operator+ (in Color lhs, in Color rhs) {
            return FromRgba(
                (byte)(lhs.R + rhs.R),
                (byte)(lhs.G + rhs.G),
                (byte)(lhs.B + rhs.B),
                (byte)(lhs.A + rhs.A)
                );
        }

        public static Color operator *(in Color lhs, in Color rhs) => FromRgba(lhs.ToVector4() * rhs.ToVector4());

        public static bool operator ==(Color lhs, Color rhs) => lhs.Value == rhs.Value;
        public static bool operator !=(Color lhs, Color rhs) => lhs.Value != rhs.Value;

        public override bool Equals(object obj) {
            if (obj == null || obj.GetType() != GetType())
                return false;

            Color lhs = this;
            Color rhs = (Color)obj;
            return lhs.Value == rhs.Value && lhs.Value == rhs.Value;
        }

        public override int GetHashCode() => Value.GetHashCode();
    }
}
