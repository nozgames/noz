/*
  NozEngine Library

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

    [TypeConverter(typeof(ThicknessTypeConverter))]
    public struct Thickness {
        public static readonly Thickness Empty = new Thickness(0);

        public float bottom;
        public float left;
        public float top;
        public float right;

        public Thickness(float value) {
            bottom = left = right = top = value;
        }

        public Thickness(float left, float top, float right, float bottom) {
            this.left = left;
            this.top = top;
            this.bottom = bottom;
            this.right = right;
        }

        public Thickness(float horizontal, float vertical) {
            left = right = horizontal;
            top = bottom = vertical;
        }

        public bool IsZero {
            get {
                return 0f == bottom && 0f == top && 0f == left && 0f == right;
            }
        }

        public static Thickness Parse(string value) {
            string[] parts = value.Split(',');
            if (null == parts || parts.Length == 0)
                return new Thickness();

            if (parts.Length == 1)
                return new Thickness(float.Parse(parts[0]));

            if (parts.Length == 2)
                return new Thickness(float.Parse(parts[0]), float.Parse(parts[1]));

            if (parts.Length == 3)
                return new Thickness(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]), 0f);

            return new Thickness(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
        }

        public override string ToString() {
            if (left == top && left == right && left == bottom)
                return $"{left}";

            if (left == right && top == bottom)
                return $"{left},{top}";

            return $"{left},{top},{right},{bottom}";
        }


        private class ThicknessTypeConverter : TypeConverter {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
                if (sourceType == typeof(string)) return true;
                return base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
                if (value == null) return Empty;
                if (value is string) return Parse((string)value);
                return base.ConvertFrom(context, culture, value);
            }
        }
    };
}

