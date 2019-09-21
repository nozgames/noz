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

namespace NoZ {

    public struct Matrix3 {
        public static readonly Matrix3 Identity = new Matrix3(1f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 1f);

        public float M11;
        public float M12;
        public float M13;
        public float M21;
        public float M22;
        public float M23;
        public float M31;
        public float M32;
        public float M33;

        public Matrix3(float m11, float m12, float m13, float m21, float m22, float m23, float m31, float m32, float m33) {
            M11 = m11;
            M12 = m12;
            M13 = m13;
            M21 = m21;
            M22 = m22;
            M23 = m23;
            M31 = m31;
            M32 = m32;
            M33 = m33;
        }

        public Matrix3 Transpose() {
            return new Matrix3(M11, M21, M31, M12, M22, M32, M13, M23, M33);
        }

        public static Matrix3 Multiply (in Matrix3 lhs, in Matrix3 rhs) {
            Matrix3 result;
            result.M11 = lhs.M11 * rhs.M11 + lhs.M12 * rhs.M21 + lhs.M13 * rhs.M31;
            result.M12 = lhs.M11 * rhs.M12 + lhs.M12 * rhs.M22 + lhs.M13 * rhs.M32;
            result.M13 = lhs.M11 * rhs.M13 + lhs.M12 * rhs.M23 + lhs.M13 * rhs.M33;

            result.M21 = lhs.M21 * rhs.M11 + lhs.M22 * rhs.M21 + lhs.M23 * rhs.M31;
            result.M22 = lhs.M21 * rhs.M12 + lhs.M22 * rhs.M22 + lhs.M23 * rhs.M32;
            result.M23 = lhs.M21 * rhs.M13 + lhs.M22 * rhs.M23 + lhs.M23 * rhs.M33;

            result.M31 = lhs.M31 * rhs.M11 + lhs.M32 * rhs.M21 + lhs.M33 * rhs.M31;
            result.M32 = lhs.M31 * rhs.M12 + lhs.M32 * rhs.M22 + lhs.M33 * rhs.M32;
            result.M33 = lhs.M31 * rhs.M13 + lhs.M32 * rhs.M23 + lhs.M33 * rhs.M33;
            
            return result;
        }

        public static Matrix3 Multiply(in Matrix3 lhs, float rhs) {
            return new Matrix3(
                lhs.M11 * rhs,
                lhs.M12 * rhs,
                lhs.M13 * rhs,
                lhs.M21 * rhs,
                lhs.M22 * rhs,
                lhs.M23 * rhs,
                lhs.M31 * rhs,
                lhs.M32 * rhs,
                lhs.M33 * rhs
                );
        }

        public Vector2 MultiplyVector(in Vector2 v) {
            return new Vector2(
                M11 * v.x + M21 * v.y + M31,
                M12 * v.x + M22 * v.y + M32
            );
        }

        public Rect MultiplyRect(in Rect v) {
            var l = M11 * v.x + M21 * v.y + M31;
            var t = M12 * v.x + M22 * v.y + M32;
            var r = M11 * (v.x+v.width) + M21 * (v.y+v.height) + M31;
            var b = M12 * (v.x+v.width) + M22 * (v.y+v.height) + M32;
            return new Rect(l, t, r - l, b - t);
        }

        public static Matrix3 Scale(float x, float y) {
            return new Matrix3(x, 0.0f, 0.0f, 0.0f, y, 0.0f, 0.0f, 0.0f, 1.0f);
        }

        public static Matrix3 Scale(Vector2 scale) {
            return new Matrix3(scale.x, 0.0f, 0.0f, 0.0f, scale.y, 0.0f, 0.0f, 0.0f, 1.0f);
        }

        public static Matrix3 Translate(float x, float y) {
            return new Matrix3(1.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, x, y, 1.0f);
        }

        public static Matrix3 Translate(Vector2 trans) {
            return new Matrix3(1.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, trans.x, trans.y, 1.0f);
        }

        public static Matrix3 Rotate(float angle) {
            float s = MathEx.Sin(angle);
            float c = MathEx.Cos(angle);
            return new Matrix3 (c, -s, 0.0f, s, c, 0.0f, 0.0f, 0.0f, 1.0f);
        }

        public float Determinant() {
            return M11 * M22 * M33 + M12 * M23 * M31 + M13 * M21 * M32 - M13 * M22 * M31 - M12 * M21 * M33 - M11 * M23 * M32;
        }

        public Matrix3 Inverse() {
            float det1 = M11 * (M33 * M22 - M32 * M23);
            float det2 = M21 * (M33 * M12 - M32 * M13);
            float det3 = M31 * (M23 * M12 - M22 * M13);

            float det = 1f / (det1 - det2 + det3);

            return new Matrix3(
                (M33 * M22 - M32 * M23) * det,
                -(M33 * M12 - M32 * M13) * det,
                (M23 * M12 - M22 * M13) * det,
                -(M33 * M21 - M31 * M23) * det,
                (M33 * M11 - M31 * M13) * det,
                -(M23 * M11 - M21 * M13) * det,
                (M32 * M21 - M31 * M22) * det,
                -(M32 * M11 - M31 * M12) * det,
                (M22 * M11 - M21 * M12) * det
            );
        }
    }
}
