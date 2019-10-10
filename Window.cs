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

namespace NoZ
{
    public interface IWindowDriver
    {
        Vector2Int Size { get; }

        bool IsCursorVisible { get; set; }

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

        private static Node _activeCursor;
        private static View _cursorView;
        private static Node _cursorPosition;

        public static IWindowDriver Driver {
            get => _driver;
            set {
                _driver = value;
                _gc = GraphicsContext.Create();


                _cursorView = new View();
                _cursorView.PresentScene(new Scene());

                _cursorPosition = new Node();
                _cursorView.Scene.AddChild(_cursorPosition);
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

        public static int ReferenceSize { get; set; }

        public static Vector2Int ScaledSize {
            get {
                if (Window.ReferenceSize > 0)
                {
                    if (Window.ReferenceOrientation == Orientation.Horizontal)
                    {
                        return new Vector2Int(Window.ReferenceSize, (int)(Size.y * (Window.ReferenceSize / (float)Size.x)));
                    }
                    else
                    {
                        return new Vector2Int((int)(Size.x * (Window.ReferenceSize / (float)Size.y)), Window.ReferenceSize);
                    }
                }
                else
                {
                    return Size;
                }
            }
        }

        public static Orientation ReferenceOrientation { get; set; } = Orientation.Horizontal;

        /// <summary>
        /// Global cursor used by the game.  This cursor will be used as long
        /// as no overriding cursor is returned via MouseOver.
        /// </summary>
        public static Node Cursor { get; set; }

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

            _cursorView.Update();            
            _cursorView.Draw(_gc);
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

        /// <summary>
        /// Handle mouse over event for the entire window
        /// </summary>
        /// <param name="e"></param>
        internal static void OnMouseOver(MouseOverEvent e)
        {
            // Update the cursor position
            _cursorPosition.Position = _cursorView.Scene.WindowToScene(Input.MousePosition);

            // Cursor for this frame is either the cursor in the event or the window cursor
            var cursor = e.Cursor ?? Cursor;

            // Nothing to do if cursor hasnt changed
            if (_activeCursor == cursor)
                return;

            // If there was an existing cursor then remove it from its parent 
            if (null != _activeCursor)
                _activeCursor.RemoveFromParent();

            // Set the new active cursor
            _activeCursor = cursor;

            if (_activeCursor != null)
            {
                _cursorPosition.AddChild(_activeCursor);
                _cursorPosition.Visibility = NodeVisibility.Visible;
            }
            else
            {
                _cursorPosition.Visibility = NodeVisibility.Collapsed;
            }
        }
    }
}
