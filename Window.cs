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

using NoZ.UI;

namespace NoZ
{
    public interface IWindowDriver
    {
        Vector2Int Size { get; }

        void Show();

        void DrawBegin();

        void DrawEnd();
    }

    public static class Window
    {
        public static readonly Event<MouseButton> MouseButtonDownEvent = new Event<MouseButton>();
        public static readonly Event<MouseButton> MouseButtonUpEvent = new Event<MouseButton>();
        public static readonly Event<Vector2> MouseMoveEvent = new Event<Vector2>();
        public static readonly Event<KeyCode> KeyUpEvent = new Event<KeyCode>();
        public static readonly Event<KeyCode> KeyDownEvent = new Event<KeyCode>();

        private static List<View> _views = new List<View>();

        private static GraphicsContext _gc;

        private static IWindowDriver _driver;

        public static IWindowDriver Driver {
            get => _driver;
            set {
                _driver = value;
                _gc = GraphicsContext.Create();
            }
        }

        /// <summary>
        /// Size of the window in pixels
        /// </summary>
        public static Vector2Int Size => Driver.Size;

        /// <summary>
        /// Background color of window
        /// </summary>
        public static Color BackgroundColor = Color.Transparent;

        /// <summary>
        /// Number of views in the view stack
        /// </summary>
        public static int ViewCount => _views.Count;

        /// <summary>
        /// Return the view at the given index
        /// </summary>
        /// <param name="index">Index of view</param>
        /// <returns>View</returns>
        public static View GetViewAt(int index) => _views[index];

        public static GraphicsContext DrawBegin()
        {
            Driver.DrawBegin();

            // Begin a graphics frame
            Graphics.Driver.BeginFrame();

            _gc.Begin(Size, BackgroundColor);

            return _gc;
        }

        public static void Draw()
        {
            for (var i = 0; i < ViewCount; i++)
                GetViewAt(i).Draw(_gc);
        }

        public static void DrawEnd()
        {
            _gc.End();

            Graphics.Driver.EndFrame();

            Driver.DrawEnd();
        }

        /// <summary>
        /// Add a view to the top of the view stack
        /// </summary>
        /// <param name="view">View to add</param>
        public static void AddView (View view)
        {
            if (view.IsVisible)
                return;

            _views.Add(view);
            view.IsVisible = true;
        }
        
        /// <summary>
        /// Remove a view from the view stack
        /// </summary>
        /// <param name="view">View to remove</param>
        public static void RemoveView(View view)
        {
            if (!view.IsVisible)
                return;

            _views.Remove(view);
            view.IsVisible = false;
        }
    }
}
