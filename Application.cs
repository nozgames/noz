/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

using NoZ.Audio;
using NoZ.Graphics;
using NoZ.VFX;
using Raylib_cs;

namespace NoZ
{
    public static class Application
    {
        private const float DefaultPixelsPerUnit = 256.0f;
        
        public static float PixelsPerUnit { get; private set; }
        public static float PixelsPerUnitInv { get; private set; }
        
        public static void Initialize(string title, int screenWidth, int screenHeight)
        {
            SetPixelsPerUnit(DefaultPixelsPerUnit);

            // Initialize all resource types
            ResourceDatabase.Initialize();
            
            Raylib.SetTraceLogLevel(TraceLogLevel.Error);
            Raylib.SetConfigFlags(ConfigFlags.ResizableWindow | ConfigFlags.Msaa4xHint);
            Raylib.InitWindow(screenWidth, screenHeight, title);
            Raylib.InitAudioDevice();
            Raylib.SetTargetFPS(60);
            Raylib.SetExitKey(0);
        }

        public static void Shutdown()
        {
            ResourceDatabase.Shutdown();
            Raylib.CloseAudioDevice();
            Raylib.CloseWindow();
        }

        public static void SetPixelsPerUnit(float ppu)
        {
            PixelsPerUnit = ppu;
            PixelsPerUnitInv = 1.0f / ppu;
        }
   }
}