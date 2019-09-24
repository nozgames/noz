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

using System;
using System.Collections.Generic;

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
        public static readonly Event<Vector2> MouseMoveEvent = new Event<Vector2>();
        public static readonly Event<KeyCode> KeyUpEvent = new Event<KeyCode>();
        public static readonly Event<KeyCode> KeyDownEvent = new Event<KeyCode>();

        private View[] _views;

        public abstract IntPtr GetNativeHandle();

        public static Window Instance { get; private set; }

        public static IGraphicsDriver Graphics { get; private set; }
        public static IAudioDriver Audio { get; private set; }

        public static View[] Views => Instance._views;

        public WindowDelegate _windowDelegate;

        private GraphicsContext _gc;

        public abstract Vector2Int Size { get; }

        public Window ()
        {
            _views = new View[] { };
        }

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

            foreach (var view in _views)
                view.Frame(_gc);

            _windowDelegate?.OnEndFrame(_gc);

            _gc.End();

            Graphics.EndFrame();

            Node.ProcessDestroyedNodes();
        }

        public void AddView (View view)
        {
            Array.Resize(ref _views, _views.Length + 1);
            _views[_views.Length - 1] = view;
        }
    }
}
