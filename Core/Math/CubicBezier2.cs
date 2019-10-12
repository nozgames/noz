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

namespace NoZ
{
    public struct CubicBezier2
    {
        public Vector2 P0;
        public Vector2 P1;
        public Vector2 P2;
        public Vector2 P3;

        public CubicBezier2(in Vector2 p0, in Vector2 p1, in Vector2 p2, in Vector2 p3)
        {
            P0 = p0;
            P1 = p1;
            P2 = p2;
            P3 = p3;
        }

        public Vector2 GetPoint(float t)
        {
            float c = 1f - t;
            float bp0 = c * c * c;
            float bp1 = 3f * t * c * c;
            float bp2 = 3f * t * t * c;
            float bp3 = t * t * t;
            return new Vector2(
                P0.x * bp0 + P1.x * bp1 + P2.x * bp2 + P3.x * bp3,
                P0.y * bp0 + P1.y * bp1 + P2.y * bp2 + P3.y * bp3
            );
        }

        public Vector2 GetTangent(float t)
        {
            var q0 = P0 + (P1 - P0) * t;
            var q1 = P1 + (P2 - P1) * t;
            var q2 = P2 + (P3 - P2) * t;
            var r0 = q0 + (q1 - q0) * t;
            var r1 = q1 + (q2 - q1) * t;
            return r1 - r0;
        }

        /// <summary>
        /// Returns the closest point on the beizer curve as a parameter value.
        /// </summary>
        /// <param name="point">Reference point</param>
        /// <param name="threshold">
        /// Parameter theshold.  The lower the value of the threshold the more detailed 
        /// the result will be.
        /// </param>
        /// <returns>Parameters value that is closest to the given point [0-1]</returns>
        public float GetClosestParameter(in Vector2 point, float threshold = 0.000001f)
        {
            var seg0 = P0;
            var seg1 = P3;
            var t0 = 0.0f;
            var t1 = 1.0f;

            while (t1 - t0 > threshold)
            {
                var dist0 = (seg0 - point).MagnitudeSquared;
                var dist1 = (seg1 - point).MagnitudeSquared;
                if (dist0 < dist1)
                {
                    t1 = (t0 + t1) * 0.5f;
                    seg1 = GetPoint(t1);
                }
                else
                {
                    t0 = (t0 + t1) * 0.5f;
                    seg0 = GetPoint(t0);
                }
            }

            return (t0 + t1) * 0.5f;
        }

        public Vector2 GetClosestPoint(in Vector2 point, float threshold)
        {
            return GetPoint(GetClosestParameter(point, threshold));
        }

        public float GetDistanceSquared(in Vector2 point, float threshold)
        {
            return (point - GetPoint(GetClosestParameter(point, threshold))).MagnitudeSquared;
        }

        public float GetDistance(in Vector2 point, float threshold)
        {
            return (point - GetPoint(GetClosestParameter(point, threshold))).Magnitude;
        }
    }
}
