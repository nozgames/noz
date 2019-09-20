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

namespace NoZ {

    public static class MathEx {

        public static readonly float PI = (float)Math.PI;
        public static readonly float Deg2Rad = (float)(Math.PI / 180.0);
        public static readonly float Rad2Deg = (float)(180.0 / Math.PI);

        /// Next power of 2
        public static uint NextPow2 (uint value) {
            value = value - 1;
            value = value | (value >> 1);
            value = value | (value >> 2);
            value = value | (value >> 4);
            value = value | (value >> 8);
            value = value | (value >> 16);
            value = value + 1;
            return value;
        }

        public static double Mix(double a, double b, double weight) {
            return (1.0 - weight) * a + weight * b;
        }

        public static float Mix(float a, float b, float weight) {
            return (1.0f - weight) * a + weight * b;
        }

        public static float Min(float a, float b) {
            return a < b ? a : b;
        }

        public static int Min(int a, int b) {
            return a < b ? a : b;
        }

        public static long Min(long a, long b) {
            return a < b ? a : b;
        }

        public static float Max(float a, float b) {
            return a > b ? a : b;
        }

        public static int Max(int a, int b) {
            return a > b ? a : b;
        }

        public static long Max(long a, long b) {
            return a > b ? a : b;
        }

        public static double Clamp(double value, double min, double max) {
            return Math.Max(Math.Min(value, max), min);
        }

        public static float Clamp(float value, float min, float max) {
            return Max(Min(value, max), min);
        }

        public static int Clamp(int value, int min, int max) {
            return Max(Min(value, max), min);
        }

        public static long Clamp(long value, long min, long max) {
            return Max(Min(value, max), min);
        }

        public static float Cos(float angle) {
            return (float)Math.Cos(angle);
        }

        public static float Sin(float angle) {
            return (float)Math.Sin(angle);
        }

        public static float Pow(float x, float y) {
            return (float)Math.Pow(x, y);
        }

        public static float Exp(float x) {
            return (float)Math.Exp(x);
        }

        public static float Sqrt(float f) {
            return (float)Math.Sqrt(f);
        }

        public static float Floor(float f) {
            return (float)Math.Floor(f);
        }

        public static float Log(float a, float newBase) {
            return (float)Math.Log(a, newBase);
        }

        public static float Log(float f) {
            return (float)Math.Log(f);
        }

        public static float EvaluateCurve(float t, float val1, float tan1, float val2, float tan2) {
            var t2 = t * t;
            var t3 = t2 * t;

            var a = 2f * t3 - 3f * t2 + 1f;
            var b = t3 - 2f * t2 + t;
            var c = t3 - t2;
            var d = -2f * t3 + 3f * t2;

            return a * val1 + b * tan1 + c * tan2 + d * val2;
        }
    }
}
