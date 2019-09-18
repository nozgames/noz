/*
  NozEngine Library

  Copyright(c) 2015 NoZ Games, LLC

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

namespace NoZ {
    public struct Rect {
        public static readonly Rect Empty = new Rect();

        public float x;
        public float y;
        public float width;
        public float height;

        public bool IsEmpty => width == 0 && height == 0;

        public float this[int index] {
            get {
                switch (index) {
                    case 0: return x;
                    case 1: return y;
                    case 2: return width;
                    default:
                        return height;
                }
            }
            set {
                switch (index) {
                    case 0: x = value; break;
                    case 1: y = value; break;
                    case 2: width = value; break;
                    default:
                        height = value;
                        break;
                }
            }
        }

        public Rect(in Vector2 v) {
            x = v.x;
            y = v.y;
            width = 0;
            height = 0;
        }

        public Rect(in Vector2 xy, in Vector2 size) {
            x = xy.x;
            y = xy.y;
            width = size.x;;
            height = size.y;
        }

        public Rect(float x, float y, float width, float height) {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public Vector2 TopLeft {
            get {
                return new Vector2(x, y);
            }
            set {
                x = value.x;
                y = value.y;
            }
        }

        public Vector2 Size {
            get => new Vector2(width, height);
            set {
                width = value.x;
                height = value.y;
            }
        }

        public Vector2 TopRight {
            get {
                return new Vector2(x + width, y);
            }
            set {
                width = value.x - x;
                y = value.y;
            }
        }

        public Vector2 BottomLeft {
            get {
                return new Vector2(x, y + height);
            }
            set {
                x = value.x;
                height = value.y - y;
            }
        }

        public Vector2 BottomRight {
            get {
                return new Vector2(x + width, y + height);
            }
            set {
                width = value.x - x;
                height = value.y - y;
            }
        }

        public Rect Union(Rect rect) {
            float ul = Math.Min(rect.x, x);
            float ur = Math.Max(rect.x + rect.width, x + width);
            float ut = Math.Min(rect.y, y);
            float ub = Math.Max(rect.y + rect.height, y + height);
            return new Rect(ul, ut, ur - ul, ub - ut);
        }

        public Rect Union(Vector2 vector) {
            float ul = Math.Min(vector.x, x);
            float ur = Math.Max(vector.x, x);
            float ut = Math.Min(vector.y, y);
            float ub = Math.Max(vector.y, y);
            return new Rect(ul, ut, ur - ul, ub - ut);
        }

        public Rect Union(Vector3 vector) {
            float ul = Math.Min(vector.x, x);
            float ur = Math.Max(vector.x, x);
            float ut = Math.Min(vector.y, y);
            float ub = Math.Max(vector.y, y);
            return new Rect(ul, ut, ur - ul, ub - ut);
        }

        public Rect Expand (float e) {
            return new Rect(x - e, y - e, width + e * 2, height + e * 2);
        }

        public Rect Expand(float ex, float ey) {
            return new Rect(x - ex, y - ey, width + ex * 2, height + ey * 2);
        }

        public Rect Offset (in Vector2 offset)
        {
            return new Rect(x + offset.x, y + offset.y, width, height);
        }

        public Rect Offset(float ox, float oy)
        {
            return new Rect(x + ox, y + oy, width, height);
        }

        public Rect Intersection(Rect rect) {
            throw new NotImplementedException();
            /*
              if(!intersects(rect)) {
                return Rect(0,0,0,0);
              }
              return Rect (
                Math.Max(rect.left,left),
                Math.Max(rect.top,top),
                Math.Min(rect.right,right),
                Math.Min(rect.bottom,bottom)
              );
             */
        }

        public Rect Parse(string value) {
            string[] parts = value.Split(',');

            Rect result = new Rect();

            if (parts.Length > 0)
                result.x = float.Parse(parts[0]);
            if (parts.Length > 1)
                result.y = float.Parse(parts[1]);
            if (parts.Length > 2)
                result.width = float.Parse(parts[2]);
            if (parts.Length > 3)
                result.height = float.Parse(parts[3]);

            return result;
        }

        public bool Contains(Vector2 point) {
            return point.x >= x && point.y >= y && point.x <= (x + width) && point.y <= (y + height);
        }

        public bool Intersects(Rect rect) {
            if (x > rect.x + rect.width)
                return false;
            if (rect.x > x + width)
                return false;
            if (y > rect.y + rect.height)
                return false;
            if (rect.y > y + height)
                return false;
            return true;
        }

        public override string ToString () {
            return $"{x},{y},{width},{height}";
        }

        public static bool operator ==(Rect lhs, Rect rhs) {
            return lhs.x == rhs.x && lhs.y == rhs.y && lhs.width == rhs.width && lhs.height == rhs.height;
        }

        public static bool operator !=(Rect lhs, Rect rhs) {
            return !(lhs.x == rhs.x && lhs.y == rhs.y && lhs.width == rhs.width && lhs.height == rhs.height);
        }


        public override bool Equals(object obj) {
            if (obj == null || obj.GetType() != GetType())
                return false;

            ref var lhs = ref this;
            var rhs = (Rect)obj;
            return lhs.x == rhs.x && lhs.y == rhs.y && lhs.width == rhs.width && lhs.height == rhs.height;
        }

        public override int GetHashCode() {
            return x.GetHashCode() ^ y.GetHashCode();
        }
    }
}




