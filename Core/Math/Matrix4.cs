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

namespace NoZ {

    public struct Matrix4 {
        public static readonly Matrix4 Identity = new Matrix4(
            1f, 0f, 0f, 0f, 
            0f, 1f, 0f, 0f, 
            0f, 0f, 1f, 0f,
            0f, 0f, 0f, 1f);

        public float M11;
        public float M12;
        public float M13;
        public float M14;
        public float M21;
        public float M22;
        public float M23;
        public float M24;
        public float M31;
        public float M32;
        public float M33;
        public float M34;
        public float M41;
        public float M42;
        public float M43;
        public float M44;


        public Matrix4(
            float m11, float m12, float m13, float m14,
            float m21, float m22, float m23, float m24,
            float m31, float m32, float m33, float m34,
            float m41, float m42, float m43, float m44
            ) {
            M11 = m11; M12 = m12; M13 = m13; M14 = m14;
            M21 = m21; M22 = m22; M23 = m23; M24 = m24;
            M31 = m31; M32 = m32; M33 = m33; M34 = m34;
            M41 = m41; M42 = m42; M43 = m43; M44 = m44;
        }

        public Matrix4 Transpose() {
            return new Matrix4(M11, M21, M31, M41, M12, M22, M32, M42, M13, M23, M33, M43, M14, M24, M34, M44);
        }

        public static Matrix4 CreateOrtho(float left, float right, float bottom, float top, float near, float far) {
            return new Matrix4(
                2.0f / (right - left),
                0.0f,
                0.0f,
                0.0f,

                0.0f,
                2.0f / (top - bottom),
                0.0f,
                0.0f,

                0.0f,
                0.0f,
                -2.0f / (far - near),
                0.0f,

                -((right + left) / (right - left)),
                -((top + bottom) / (top - bottom)),
                -((far + near) / (far - near)),
                1.0f
            );
        }
    }
}

/*

    Matrix4& Transpose(void) {
    *this = Transposed();
    return *this;
}


Matrix4& Scale(float x, float y) {
    Matrix4 scale(x, 0.0f, 0.0f, 0.0f, y, 0.0f, 0.0f, 0.0f, 1.0f);
    *this = *this * scale;
    return *this;
}

Matrix4& Scale(const Vector2& v) {
    return Scale(v.x, v.y);
}


Matrix4& Rotate(float angle) {
    float s = Math::Sin(angle);
    float c = Math::Cos(angle);
    Matrix4 rot(c, -s, 0.0f, s, c, 0.0f, 0.0f, 0.0f, 1.0f);
    *this = *this * rot;
    return *this;
}

const float& operator[] (int i) const { return d[i]; }

		float& operator[] (int i) {
    return d[i];
}

Matrix4 operator *(const Matrix4& m) const;

Matrix4& operator*= (const Matrix4& m) {
    *this = *this * m;
    return *this;
}

Vector2 operator *(const Vector2& v) const {
			return Vector2(
                d[0] * v.x + d[3] * v.y + d[6],
                d[1] * v.x + d[4] * v.y + d[7]

            );
		}

		Vector2 InverseTransform(const Vector2& point) const;

void Rotate(float i_x, float i_y, float i_z, float i_sin, float i_cos, float i_inv_cos);

Matrix4& Invert(void) {
    *this = Inverted();
    return *this;
}

Matrix4 Inverted(void) const; 
}
*/