/*
  NozEngine Library

  Colin Green, January 2005
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

namespace NoZ  {
    public struct Random {
        private const double UInt32ToDouble = 1.0 / ((double)int.MaxValue + 1.0);
        private const uint R1 = 842502087;
        private const uint R2 = 3579807591;
        private const uint R3 = 273326509;

        private uint _r0;
        private uint _r1;
        private uint _r2;
        private uint _r3;
        private int _seed;

        public Random (int seed) {
            _seed = seed;
            _r0 = (uint)_seed;
            _r1 = R1;
            _r2 = R2;
            _r3 = R3;
        }

        public int Seed {
            get => _seed;
            set {
                _seed = value;
                _r0 = (uint)value;
                _r1 = R1;
                _r2 = R2;
                _r3 = R3;
            }
        }

        public int Next ( ) {
            uint t = (_r0 ^ (_r0 << 11));
            _r0 = _r1;
            _r1 = _r2;
            _r2 = _r3;
            _r3 = (_r3 ^ (_r3 >> 19)) ^ (t ^ (t >> 8));

            // Handle the special case where the value int.MaxValue is generated. This is outside of 
            // the range of permitted values, so we therefore call Next() to try again.
            uint rtn = _r3 & 0x7FFFFFFF;
            if (rtn == 0x7FFFFFFF)
                return Next();

            return (int)rtn;
        }

        /// <summary>
        /// Returns a random value that is less than the given maximum.
        /// </summary>
        /// <param name="max">The exclusive upper bound of the random value.</param>
        /// <returns>A random integer greater than or equal to zero and less than the maximum value.</returns>
        public int Next(int max) {
            return Next() % max;
        }

        /// <summary>
        /// Returns a random value within the specified range.
        /// </summary>
        /// <param name="min">The inclusive lower bound of the random value.</param>
        /// <param name="max">The exclusive upper bound of the random value.</param>
        /// <returns>A random integer greater than or equal to the min value and less than the max value.</returns>
        public int Next(int min, int max) {
            return min + (Next() % (max-min));
        }

        /// <summary>
        /// Generates a random double. Values returned are from 0.0 up to but not including 1.0.
        /// </summary>
        /// <returns></returns>
        public double NextDouble() {
            uint t = (_r0 ^ (_r0 << 11));
            _r0 = _r1;
            _r1 = _r2;
            _r2 = _r3;

            // Here we can gain a 2x speed improvement by generating a value that can be cast to 
            // an int instead of the more easily available uint. If we then explicitly cast to an 
            // int the compiler will then cast the int to a double to perform the multiplication, 
            // this final cast is a lot faster than casting from a uint to a double. The extra cast
            // to an int is very fast (the allocated bits remain the same) and so the overall effect 
            // of the extra cast is a significant performance improvement.
            //
            // Also note that the loss of one bit of precision is equivalent to what occurs within 
            // System.Random.
            return (UInt32ToDouble * (int)(0x7FFFFFFF & (_r3 = (_r3 ^ (_r3 >> 19)) ^ (t ^ (t >> 8)))));
        }
    }
}

