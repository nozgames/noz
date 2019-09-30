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

using NoZ.UI;

namespace NoZ
{
    public class ApplicationDelegate : Object
    {
        /// <summary>
        /// Called when the application is first started after all
        /// drivers have been initialized and the window created.
        /// </summary>
        public virtual void Start () { }

        public virtual void BecomeActive() { }

        public virtual void ResignActive() { }

        public virtual void DrawBegin (GraphicsContext context) { }

        public virtual void DrawEnd (GraphicsContext context) { }

    }

    public static class Application
    {
        private static ApplicationDelegate _applicationDelegate;

        private static MouseOverEvent _mouseOverEvent = new MouseOverEvent();
        private static MouseButtonEvent _mouseButtonEvent = new MouseButtonEvent();
        private static MouseWheelEvent _mouseWheelEvent = new MouseWheelEvent();
        private static KeyboardEvent _keyboardEvent = new KeyboardEvent();

        /// <summary>
        /// Initialize the application
        /// </summary>
        /// <param name="applicationDelegate"></param>
        public static void Initialize (ApplicationDelegate applicationDelegate)
        {
            _applicationDelegate = applicationDelegate;

            // Initialize the input system
            Input.Initialize();

            // Initialize the animation system.
            Animation.Initialize();
        }

        public static void Step ()
        {
            // Advance time
            Time.Step();

            // Update the mouse overs for all nodes in the scenes container.            
            Input.BeginFrame();

            GenerateEvents();

            // Update all 
            for(var i=0;i<Window.ViewCount;i++)
                Window.GetViewAt(i).Update();

            Animation.Step(AnimationUpdateMode.Update);

            var gc = Window.DrawBegin();

            _applicationDelegate.DrawBegin(gc);

            Window.Draw();

            _applicationDelegate.DrawEnd(gc);

            Window.DrawEnd();

            Node.ProcessDestroyedNodes();

            Input.EndFrame();
        }

        private static void GenerateEvents()
        {
            // TODO: Determine which node the mouse is over

#if false
            // Mouse over
            GenerateMouseOverEvent();
#endif

            // Mouse buttons ?
            GenerateMouseButtonEvent(MouseButton.Left);
            GenerateMouseButtonEvent(MouseButton.Right);
            GenerateMouseButtonEvent(MouseButton.Middle);

#if false
            // Mouse wheel
            GenerateMouseWheelEvent();

            // Keyboard envents
            GenerateKeyboardEvents();
#endif
        }

        private static void GenerateMouseButtonEvent(MouseButton button)
        {
            if (Input.WasButtonPressed(button))
                GenerateMouseButtonEvent(button, true);

            if (Input.WasButtonReleased(button))
                GenerateMouseButtonEvent(button, false);
        }

        private static void GenerateMouseButtonEvent(MouseButton button, bool down)
        {
            _mouseButtonEvent.Reset();
            _mouseButtonEvent.Button = button;
            _mouseButtonEvent.IsDown = down;
            _mouseButtonEvent.Position = Input.MousePosition;

            if (Control.GetCapture() != null)
            {
                if (down)
                    Control.GetCapture().OnMouseDown(_mouseButtonEvent);
                else
                    Control.GetCapture().OnMouseUp(_mouseButtonEvent);
            }
            else
            {
                for (var node = Input.MouseOver;
                    node != null && !_mouseButtonEvent.IsHandled;
                    node = node.Parent)
                {
                    if (down)
                        node.OnMouseDown(_mouseButtonEvent);
                    else
                        node.OnMouseUp(_mouseButtonEvent);
                }
            }
        }
    }
}
