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

namespace NoZ
{

    public static class MathEx
    {

        public static readonly float PI = (float)Math.PI;
        public static readonly float Deg2Rad = (float)(Math.PI / 180.0);
        public static readonly float Rad2Deg = (float)(180.0 / Math.PI);

        /// Next power of 2
        public static uint NextPow2(uint value)
        {
            value = value - 1;
            value = value | (value >> 1);
            value = value | (value >> 2);
            value = value | (value >> 4);
            value = value | (value >> 8);
            value = value | (value >> 16);
            value = value + 1;
            return value;
        }

        public static double Mix(double a, double b, double weight) => (1.0 - weight) * a + weight* b;

        public static float Mix(float a, float b, float weight) => (1.0f - weight) * a + weight * b;

        public static float Min(float a, float b) => a < b ? a : b;

        public static int Min(int a, int b) => a < b ? a : b;

        public static long Min(long a, long b) => a < b ? a : b;

        public static float Max(float a, float b) => a > b ? a : b;

        public static int Max(int a, int b) => a > b ? a : b;

        public static long Max(long a, long b) => a > b? a : b;

        public static double Clamp(double value, double min, double max) => Math.Max(Math.Min(value, max), min);

        public static float Clamp(float value, float min, float max) => Max(Min(value, max), min);

        public static int Clamp(int value, int min, int max) => Max(Min(value, max), min);

        public static long Clamp(long value, long min, long max) => Max(Min(value, max), min);

        public static float Cos(float angle) => (float)Math.Cos(angle);

        public static float Sin(float angle) => (float)Math.Sin(angle);

        public static float Pow(float x, float y) => (float)Math.Pow(x, y);

        public static float Exp(float x) => (float)Math.Exp(x);

        public static float Sqrt(float f) => (float)Math.Sqrt(f);

        public static float Floor(float f) => (float)Math.Floor(f);

        public static float Log(float a, float newBase) => (float)Math.Log(a, newBase);

        public static float Log(float f) => (float)Math.Log(f);

        public static float EvaluateCurve(float t, float val1, float tan1, float val2, float tan2)
        {
            var t2 = t * t;
            var t3 = t2 * t;

            var a = 2f * t3 - 3f * t2 + 1f;
            var b = t3 - 2f * t2 + t;
            var c = t3 - t2;
            var d = -2f * t3 + 3f * t2;

            return a * val1 + b * tan1 + c * tan2 + d * val2;
        }

        static readonly int[] PerlinPermutation = new int[512] {
            151,160,137,91,90,15,
            131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
            190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
            88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
            77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
            102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
            135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
            5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
            223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
            129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
            251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
            49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
            138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180,

            151,160,137,91,90,15,
            131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
            190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
            88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
            77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
            102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
            135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
            5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
            223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
            129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
            251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
            49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
            138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180

        };

        private static readonly Vector2[] PerlinGradients;

        static MathEx()
        {
            Vector2[] Grad =
            {
                new Vector2( 1,  1),
                new Vector2(-1,  1),
                new Vector2( 1, -1),
                new Vector2(-1, -1)
            };

            PerlinGradients = new Vector2[512];
            for (var i = 0; i < 256; i++)
                PerlinGradients[i] = PerlinGradients[i + 256] = Grad[PerlinPermutation[i] % 4];
        }

        static readonly float F2 = 0.5f * (Sqrt(3) - 1);
        static readonly float G2 = (3 - Sqrt(3)) / 6;

        static public float PerlinNoise(float xin, float yin)
        {
            // Skew the input space to determine which simplex cell we're in
            var s = (xin + yin) * F2; // Hairy factor for 2D
            var i = (int)Floor(xin + s);
            var j = (int)Floor(yin + s);
            var t = (i + j) * G2;

            // The x,y distances from the cell origin, unskewed.
            var x0 = xin - i + t; 
            var y0 = yin - j + t;

            // Determine which triangle we are in.
            // x0 > y0  : lower triangle, XY order: (0,0)->(1,0)->(1,1)
            // x0 <= y0 : upper triangle, YX order: (0,0)->(0,1)->(1,1)
            var i1 = x0 > y0 ? 1 : 0;
            var j1 = x0 > y0 ? 0 : 1;

            // A step of (1,0) in (i,j) means a step of (1-c,-c) in (x,y), and
            // a step of (0,1) in (i,j) means a step of (-c,1-c) in (x,y), where
            // c = (3-sqrt(3))/6
            var x1 = x0 - i1 + G2; // Offsets for middle corner in (x,y) unskewed coords
            var y1 = y0 - j1 + G2;
            var x2 = x0 - 1 + 2 * G2; // Offsets for last corner in (x,y) unskewed coords
            var y2 = y0 - 1 + 2 * G2;
            
            // Work out the hashed gradient indices of the three simplex corners
            i &= 255;
            j &= 255;

            var gi0 = PerlinGradients[i + PerlinPermutation[j]];
            var gi1 = PerlinGradients[i + i1 + PerlinPermutation[j + j1]];
            var gi2 = PerlinGradients[i + 1 + PerlinPermutation[j + 1]];

            // Calculate the contribution from the three corners
            var t0 = 0.5f - x0 * x0 - y0 * y0;
            var n0 = t0 < 0 ? 0 : t0 * t0 * t0 * t0 * Vector2.Dot(gi0, new Vector2(x0, y0));

            var t1 = 0.5f - x1 * x1 - y1 * y1;
            var n1 = t1 < 0 ? 0 : t1 * t1 * t1 * t1 * Vector2.Dot(gi1, new Vector2(x1, y1));

            var t2 = 0.5f - x2 * x2 - y2 * y2;
            var n2 = t2 < 0 ?  0 : t2 * t2 * t2 * t2 * Vector2.Dot(gi2, new Vector2(x2, y2));

            // Add contributions from each corner to get the final noise value.
            // The result is scaled to return values in the interval [-1,1].
            return 70.0f * (n0 + n1 + n2);
        }
    }
}
