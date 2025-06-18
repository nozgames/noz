/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

using NoZ.Audio;
using NoZ.Graphics;
using NoZ.VFX;
using SDL3;

namespace NoZ
{
    public static class Application
    {
        private const float DefaultPixelsPerUnit = 256.0f;
        private static nint _window;
        private static bool _isRunning = true;
        private static int _screenWidth;
        private static int _screenHeight;

        public static Renderer Renderer { get; private set; }
        public static float PixelsPerUnit { get; private set; }
        public static float PixelsPerUnitInv { get; private set; }
        public static bool IsRunning => _isRunning;

        public static void Initialize(string title, int screenWidth, int screenHeight)
        {
            SetPixelsPerUnit(DefaultPixelsPerUnit);
            ResourceDatabase.Initialize();
            SDL.Init(
                SDL.InitFlags.Video |
                SDL.InitFlags.Audio |
                SDL.InitFlags.Gamepad);
            _window = SDL.CreateWindow(
                title,
                screenWidth,
                screenHeight,
                SDL.WindowFlags.Resizable);
            Renderer = new Renderer();
            Renderer.Load(_window);
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;
        }

        public static void Update()
        {
            SDL.Event evt;
            while (SDL.PollEvent(out evt))
            {
                if (evt.Type == (uint)SDL.EventType.Quit)
                    _isRunning = false;
            }
        }

        public static void Shutdown()
        {
            Renderer.Unload();
            SDL.DestroyWindow(_window);
            ResourceDatabase.Shutdown();
            SDL.Quit();
        }

        public static void SetPixelsPerUnit(float ppu)
        {
            PixelsPerUnit = ppu;
            PixelsPerUnitInv = 1.0f / ppu;
        }
    }
}