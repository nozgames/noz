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

using System.Diagnostics;

namespace NoZ
{
    public static class Time
    {
        private static Stopwatch _stopwatch;
        private static long _elapsed;
        private static float _deltaTime;
        private static float _unscaledDeltaTime;
        private const float MillisecondsToSeconds = 1.0f / 1000.0f;

        static Time()
        {
            _stopwatch = Stopwatch.StartNew();
            _elapsed = 0;
            TotalTime = 0;
            _deltaTime = 0.0f;
        }

        /// <summary>
        /// Scale at which time runs
        /// </summary>
        public static float TimeScale { get; set; } = 1.0f;

        /// <summary>
        /// True if a fixed time step is being run
        /// </summary>
        public static bool InFixedTimeStep { get; private set; } = false;

        /// <summary>
        /// Time in seconds it took to complete the last frame scaled by the timescale
        /// </summary>
        public static float DeltaTime => InFixedTimeStep ? FixedDeltaTime : _deltaTime;

        /// <summary>
        /// Time in seconds it took to complete the last frame.
        /// </summary>
        public static float UnscaledDeltaTime => InFixedTimeStep ? FixedUnscaledDeltaTime : _unscaledDeltaTime;

        /// <summary>
        /// Time in seconds for an unscaled time step
        /// </summary>
        public static float FixedUnscaledDeltaTime { get; set; } = 1.0f / 60.0f;

        /// <summary>
        /// Time in seconds for the current scaled fixed time
        /// </summary>
        public static float FixedDeltaTime { get; private set; }

        /// <summary>
        /// Time in seconds since the game started.
        /// </summary>
        public static float TotalTime { get; private set; }

        internal static void Step()
        {
            var temp = _stopwatch.ElapsedMilliseconds;
            var delta = temp - _elapsed;
            _elapsed = temp;

            // Unscaled delta time is the time before timescale is applied
            _unscaledDeltaTime = ((int)delta) * MillisecondsToSeconds;

            // Apply timescale
            _deltaTime = _unscaledDeltaTime * TimeScale;

            // Keep try of total time since game started
            TotalTime = ((int)_elapsed) * MillisecondsToSeconds;
        }

        /// <summary>
        /// Begin a new fixed time step
        /// </summary>
        internal static void BeginFixedTimeStep ()
        {
            InFixedTimeStep = true;
            FixedDeltaTime = TimeScale * FixedUnscaledDeltaTime;
        }

        /// <summary>
        /// End a a fixed time step
        /// </summary>
        internal static void EndFixedTimeStep ()
        {
            InFixedTimeStep = false;
        }
    }
}
