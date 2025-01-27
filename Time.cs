/*

    Copyright (c) 2024 NoZ Games, LLC. All rights reserved.

*/

using Raylib_cs;

namespace NoZ
{
    public static class Time 
    {
        private static float _savedDeltaTime;

        public static float DeltaTime { get; private set; }
        public static float UnscaledDeltaTime { get; private set; }
        public static float TimeScale { get; set; } = 1.0f;
        public static ulong FrameIndex { get; private set; } = 1;

        public static bool IsFixedStep { get; private set; }

        public static void Update()
        {
            FrameIndex++;
            UnscaledDeltaTime = Raylib.GetFrameTime(); 
            DeltaTime = Raylib.GetFrameTime() * TimeScale; 
        }

        public static void Clear()
        {
            DeltaTime = 0;
        }

        public static void BeginFixed(float fixedDeltaTime)
        {
            if (IsFixedStep)
                return;

            IsFixedStep = true;
            _savedDeltaTime = DeltaTime;
            DeltaTime = fixedDeltaTime;
        }

        public static void EndFixed()
        {
            DeltaTime = _savedDeltaTime;
            IsFixedStep = false;
        }
    }
}
