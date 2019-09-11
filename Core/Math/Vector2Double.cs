using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;

namespace NoZ {
    public class Vector2DoubleTypeConverter : TypeConverter {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
            if (sourceType == typeof(string)) {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
            if (value == null) {
                return Vector2Double.Zero;
            }

            if (value is string) {
                return Vector2Double.Parse((string)value);
            }

            return base.ConvertFrom(context, culture, value);
        }

        /*
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
            if (destinationType == typeof(string)) {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo( ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
            if (value != null) {
                if (!(value is Author)) {
                    throw new ArgumentException(
                        "Invalid Author", "value");
                }
            }

            if (destinationType == typeof(string)) {
                if (value == null) {
                    return String.Empty;
                }

                Author auth = (Author)value;

                if (auth.MiddleName != String.Empty) {
                    return String.Format("{0} {1} {2}",
                        auth.FirstName,
                        auth.MiddleName,
                        auth.LastName);
                } else {
                    return String.Format("{0} {1}",
                         auth.FirstName,
                        auth.LastName);
                }
            }

            return base.ConvertTo(context, culture, value,
                destinationType);
        }
        */
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    [TypeConverter(typeof(Vector2DoubleTypeConverter))]
    public struct Vector2Double {
        public static readonly Vector2Double Zero = new Vector2Double();
        public static readonly Vector2Double One = new Vector2Double(1f);
        public static readonly Vector2Double Half = new Vector2Double(0.5f);
        public static readonly Vector2Double OneZero = new Vector2Double(1f, 0f);
        public static readonly Vector2Double ZeroOne = new Vector2Double(0f, 1f);
        public static readonly Vector2Double NaN = new Vector2Double(double.NaN, double.NaN);
        public static readonly Vector2Double Infinity = new Vector2Double(double.PositiveInfinity, double.PositiveInfinity);
        public static readonly int SizeInBytes = Marshal.SizeOf(typeof(Vector2Double));

        public double x;
        public double y;

        public Vector2Double(double x, double y) {
            this.x = x;
            this.y = y;
        }

        public Vector2Double(double v) {
            x = y = v;
        }

        public double Magnitude => Math.Sqrt(x * x + y * y);

        /// Dot product
        public static double Dot(Vector2Double lhs, Vector2Double rhs) => lhs.x * rhs.x + lhs.y * rhs.y;

        public static double Cross(Vector2Double lhs, Vector2Double rhs) => (lhs.x * rhs.y) - (lhs.y * rhs.x);

        /// Returns the minimum component of the vector
        public double Min() => Math.Min(x, y);

        /// Returns the maximum component of the vector
        public double Max() => Math.Max(x, y);

        public Vector2Double Normalized {
            get {
                double l = Magnitude;
                return new Vector2Double(x / l, y / l);
            }
        }

        public Vector2Double NormalizedSafe {
            get {
                double l = Magnitude;

                // Prevent divide by zero crashes
                if (l == 0)
                    return ZeroOne;

                return new Vector2Double(x / l, y / l);
            }
        }

        public Vector2Double OrthoNormalize(bool polarity = true) {
            double len = Magnitude;
            return (polarity ? 1.0 : -1.0) * new Vector2Double(-y / len, x / len);
        }

        public Vector2Double OrthoNormalizeSafe (bool polarity = true) {
            double len = Magnitude;
            if (len == 0)
                return (polarity ? 1.0 : -1.0) * ZeroOne;
            return (polarity ? 1.0 : -1.0) * new Vector2Double(-y / len, x / len);
        }

        public static Vector2Double Mix(Vector2Double a, Vector2Double b, double weight) {
            return new Vector2Double(
                MathEx.Mix(a.x, b.x, weight),
                MathEx.Mix(a.y, b.y, weight)
                );
        }

        /// Return a vector that contains the maxium values of both components
        public static Vector2Double Max(Vector2Double a, Vector2Double b) {
            return new Vector2Double(Math.Max(a.x, b.x), Math.Max(a.y, b.y));
        }

        /// Return a vector that contains the minimum values of both components
        public static Vector2Double Min(Vector2Double a, Vector2Double b) {
            return new Vector2Double(Math.Min(a.x, b.x), Math.Min(a.y, b.y));
        }

        public static Vector2Double Clamp(Vector2Double v, Vector2Double min, Vector2Double max) {
            return new Vector2Double(MathEx.Clamp(v.x, min.x, max.x), MathEx.Clamp(v.y, min.y, max.y));
        }

        public static Vector2Double operator +(Vector2Double lhs, Vector2Double rhs) {
            return new Vector2Double(lhs.x + rhs.x, lhs.y + rhs.y);
        }

        public static Vector2Double operator *(Vector2Double lhs, Vector2Double rhs) {
            return new Vector2Double(lhs.x * rhs.x, lhs.y * rhs.y);
        }

        public static Vector2Double operator *(Vector2Double lhs, double rhs) {
            return new Vector2Double(lhs.x * rhs, lhs.y * rhs);
        }

        public static Vector2Double operator *(double lhs, Vector2Double rhs) {
            return new Vector2Double(rhs.x * lhs, rhs.y * lhs);
        }

        public static Vector2Double operator -(Vector2Double lhs, Vector2Double rhs) {
            return new Vector2Double(lhs.x - rhs.x, lhs.y - rhs.y);
        }

        public static Vector2Double operator /(Vector2Double lhs, Vector2Double rhs) {
            return new Vector2Double(lhs.x / rhs.x, lhs.y / rhs.y);
        }

        public static Vector2Double operator /(Vector2Double lhs, double rhs) {
            return new Vector2Double(lhs.x / rhs, lhs.y / rhs);
        }

        public static bool operator ==(Vector2Double lhs, Vector2Double rhs) {
            return lhs.x == rhs.x && lhs.y == rhs.y;
        }

        public static bool operator !=(Vector2Double lhs, Vector2Double rhs) {
            return !(lhs.x == rhs.x && lhs.y == rhs.y);
        }

        public override bool Equals(object obj) {
            if (obj == null || obj.GetType() != GetType())
                return false;

            Vector2Double lhs = this;
            Vector2Double rhs = (Vector2Double)obj;
            return lhs.x == rhs.x && lhs.y == rhs.y;
        }

        public override int GetHashCode() {
            return x.GetHashCode() ^ y.GetHashCode();
        }

        public static Vector2Double Parse(string value) {
            string[] parts = value.Split(',');
            if (null == parts || parts.Length == 0)
                return Vector2Double.Zero;

            if (parts.Length == 1)
                return new Vector2Double(double.Parse(parts[0]));

            return new Vector2Double(double.Parse(parts[0]), double.Parse(parts[1]));
        }

        public override string ToString() {
            if (x == y)
                return $"{x}";

            return $"{x},{y}";
        }

        public Vector2 ToVector2() => new Vector2((float)x, (float)y);
    }
}
