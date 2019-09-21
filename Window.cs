using System;

using NoZ.Graphics;

namespace NoZ
{
    public class WindowDelegate
    {
        public virtual void OnCreated() { }

        public virtual void OnBecomeActive() { }

        public virtual void OnResignActive() { }

        public virtual void OnBeginFrame(GraphicsContext context) { }

        public virtual void OnEndFrame(GraphicsContext context) { }
    }

    public abstract class Window
    {
        public abstract IntPtr GetNativeHandle();

        public static Window Instance { get; private set; }

        public static IGraphicsDriver Graphics { get; private set; }
        public static IAudioDriver Audio { get; private set; }

        public WindowDelegate _windowDelegate;

        public abstract Vector2Int Size { get; }

        public static T Create<T>(WindowDelegate windowDelegate, IGraphicsDriver graphics, IAudioDriver audio) where T : Window
        {
            var window = Activator.CreateInstance<T>();
            window._windowDelegate = windowDelegate;
            Graphics = graphics;
            Audio = audio;
            Instance = window;
            window._windowDelegate?.OnCreated();
            return window;
        }

        public virtual void Frame()
        {
            Graphics.BeginFrame();

            var context = Graphics.CreateContext();
            context.Begin(Size, Color.Red);

            _windowDelegate?.OnBeginFrame(context);

            // TODO: views

            _windowDelegate?.OnEndFrame(context);

            context.End();

            Graphics.EndFrame();
        }
    }
}
