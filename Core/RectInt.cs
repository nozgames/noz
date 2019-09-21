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

namespace NoZ {
    public struct RectInt {
        public static readonly RectInt Empty = new RectInt();

        public int x;
        public int y;
        public int width;
        public int height;

        public RectInt(int x, int y, int width, int height) {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public bool IsEmpty => width == 0 && height == 0;

        public int this[int index] {
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

        public Vector2Int TopLeft {
            get { return new Vector2Int(x, y); }
            set {
                x = value.x;
                y = value.y;
            }
        }

        public Vector2Int TopRight {
            get { return new Vector2Int(x + width, y); }
            set {
                width = value.x - x;
                y = value.y;
            }
        }

        public Vector2Int BottomLeft {
            get { return new Vector2Int(x, y + height); }
            set {
                x = value.x;
                height = value.y - y;
            }
        }

        public Vector2Int BottomRight {
            get { return new Vector2Int(x + width, y + height); }
            set {
                width = value.x - x;
                height = value.y - y;
            }
        }

        public Vector2Int Size => new Vector2Int(width, height);

        public RectInt Union(RectInt RectInt) {
            int ul = Math.Min(RectInt.x, x);
            int ur = Math.Max(RectInt.x + RectInt.width, x + width);
            int ut = Math.Min(RectInt.y, y);
            int ub = Math.Max(RectInt.y + RectInt.height, y + height);
            return new RectInt(ul, ut, ur - ul, ub - ut);
        }

        public RectInt Union(Vector2Int vector) {
            int ul = Math.Min(vector.x, x);
            int ur = Math.Max(vector.x, x);
            int ut = Math.Min(vector.y, y);
            int ub = Math.Max(vector.y, y);
            return new RectInt(ul, ut, ur - ul, ub - ut);
        }
        
        public RectInt Intersection(RectInt RectInt) {
            throw new NotImplementedException();
            /*
              if(!intersects(RectInt)) {
                return RectInt(0,0,0,0);
              }
              return RectInt (
                Math.Max(RectInt.left,left),
                Math.Max(RectInt.top,top),
                Math.Min(RectInt.right,right),
                Math.Min(RectInt.bottom,bottom)
              );
             */
        }

        public RectInt Parse(string value) {
            string[] parts = value.Split(',');

            RectInt result = new RectInt();

            if (parts.Length > 0)
                result.x = int.Parse(parts[0]);
            if (parts.Length > 1)
                result.y = int.Parse(parts[1]);
            if (parts.Length > 2)
                result.width = int.Parse(parts[2]);
            if (parts.Length > 3)
                result.height = int.Parse(parts[3]);

            return result;
        }

        public bool Contains(Vector2Int point) {
            return point.x >= x && point.y >= y && point.x <= (x + width) && point.y <= (y + height);
        }

        public bool Intersects(RectInt RectInt) {
            if (x > RectInt.x + RectInt.width)
                return false;
            if (RectInt.x > x + width)
                return false;
            if (y > RectInt.y + RectInt.height)
                return false;
            if (RectInt.y > y + height)
                return false;
            return true;
        }

        public override string ToString() {
            return $"{x},{y},{width},{height}";
        }
    }
}
