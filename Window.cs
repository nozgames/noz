using System;

using NoZ.Graphics;

namespace NoZ
{
    public class WindowDelegate : Object
    {
        public virtual void OnCreated() { }

        public virtual void OnBecomeActive() { }

        public virtual void OnResignActive() { }

        public virtual void OnBeginFrame(GraphicsContext context) { }

        public virtual void OnEndFrame(GraphicsContext context) { }
    }

    public abstract class Window
    {
        public static readonly Event<MouseButton> MouseButtonDownEvent = new Event<MouseButton>();
        public static readonly Event<MouseButton> MouseButtonUpEvent = new Event<MouseButton>();
        public static readonly Event<KeyCode> KeyUpEvent = new Event<KeyCode>();
        public static readonly Event<KeyCode> KeyDownEvent = new Event<KeyCode>();

        public abstract IntPtr GetNativeHandle();

        public static Window Instance { get; private set; }

        public static IGraphicsDriver Graphics { get; private set; }
        public static IAudioDriver Audio { get; private set; }

        public WindowDelegate _windowDelegate;

        private GraphicsContext _gc;

        public abstract Vector2Int Size { get; }

        public static T Create<T>(WindowDelegate windowDelegate, IGraphicsDriver graphics, IAudioDriver audio) where T : Window
        {
            var window = Activator.CreateInstance<T>();
            window._windowDelegate = windowDelegate;
            Graphics = graphics;
            Audio = audio;
            Instance = window;
            Input.Initialize(window);
            window._windowDelegate?.OnCreated();
            return window;
        }

        public virtual void Frame()
        {
            Graphics.BeginFrame();

            if (null == _gc)
                _gc = Graphics.CreateContext();

            _gc.Begin(Size, Color.Red);

            _windowDelegate?.OnBeginFrame(_gc);

            // TODO: views

            _windowDelegate?.OnEndFrame(_gc);

            _gc.End();

            Graphics.EndFrame();

            Node.ProcessDestroyedNodes();
        }
    }
}
