/*

    Copyright (c) 2024 NoZ Games, LLC. All rights reserved.

*/

namespace NoZ.Tweening
{
    [Flags]
    internal enum TweenFlags : ushort
    {
        None = 0,

        /// <summary>Tween will automatically stop when the time has been exceeded</summary>
        AutoStop = 1 << 1,

        /// <summary>Delta time should be unscaled</summary>
        UnscaledTime = 1 << 2,

        /// <summary>Tween should automatically loop</summary>
        Looping = 1 << 3,

        /// <summary>Play forward for the first half of the duration to the end, and in reverse for the second half to the start</summary>
        PingPong = 1 << 4,

        /// <summary>True when the elapsed time is between delay and delay + duration</summary>
        Evaluating = 1 << 5
    }
}
