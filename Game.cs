using System;

namespace NoZ
{
    public class Game
    {
        public delegate void RunDelegate();

        public static Game Instance {
            get; private set;
        }

        private IAudioDriver _audioDriver;
        private IGraphicsDriver _graphicsDriver;
        private Window _window;

        public static IAudioDriver AudioDriver { get { return Instance._audioDriver; } }
        public static IGraphicsDriver GraphicsDriver { get { return Instance._graphicsDriver; } }

        public Game (Window window, IGraphicsDriver graphicsDriver, IAudioDriver audioDriver)
        {
            _window = window;
            _graphicsDriver = graphicsDriver;
            _audioDriver = audioDriver;
            Instance = this;
        }

        public void Run ( )
        {

        }

        public void Frame ( )
        {
            _window.BeginFrame();
            _graphicsDriver.BeginFrame();

            var context = _graphicsDriver.CreateContext();
            context.Begin(new Vector2Int(800, 600), Color.Red);
            context.End();

            _graphicsDriver.EndFrame();
            _window.EndFrame();
        }
    }
}
