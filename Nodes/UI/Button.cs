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

namespace NoZ.UI
{
    public enum ClickMode
    {
        /// <summary>
        /// Issue a Click on mouse down
        /// </summary>
        Press,

        /// <summary>
        /// Issue a click on mouse up
        /// </summary>
		Release
    };

    public class Button : Control
    {
        /// <summary>
        /// Broadcast when the button is pressed down
        /// </summary>
        public static readonly Event<Button> PressEvent = new Event<Button>();

        /// <summary>
        /// Broadcast when the button is released
        /// </summary>
        public static readonly Event<Button> ReleaseEvent = new Event<Button>();

        /// <summary>
        /// Broadcast when the button is clicked
        /// </summary>
        public static readonly Event<Button> ClickEvent = new Event<Button>();

        public ClickMode ClickMode { get; set; } = ClickMode.Release;

        /// <summary>
        /// True if the button is in the pressed state
        /// </summary>
        public bool IsPressed => HasCapture && IsMouseOver;

        public Button()
        {
            IsInteractive = true;
        }

        protected internal override void OnMouseDown(MouseButtonEvent e)
        {
            // Handle non interactive state.
            if (!IsInteractive)
            {
                e.IsHandled = true;
                return;
            }

            // We only care about left ButtonBase presses
            if (e.Button != MouseButton.Left)
                return;

            // Mark the event as handled so it does not propegate further
            e.IsHandled = true;

            // Set the button as focused
            // TODO: Focus
            //if (IsFocusable) Focus();

            // Set the capture to this node to ensure all future events are routed to it.
            Capture();

            OnPress();

            // Fire click event if click mode is press
            if (ClickMode == ClickMode.Press)
                OnClick();
        }

        protected internal override void OnMouseUp(MouseButtonEvent e)
        {
            // We only care if the button still has capture
            if (!HasCapture)
                return;

            // Release event
            OnRelease();

            // If the position is within the ButtonBase...
            if (ClickMode == ClickMode.Release && IsMouseOver)
                OnClick();

            // Release capture
            ReleaseCapture();
        }

        protected override void OnReleaseCapture(bool cancelled)
        {
            base.OnReleaseCapture(cancelled);
        }

        protected override void OnMouseEnter()
        {
            base.OnMouseEnter();

            if (HasCapture)
                OnPress();
        }

        protected override void OnMouseLeave()
        {
            base.OnMouseLeave();

            if (HasCapture)
                OnRelease();
        }

        /// <summary>
        /// Handle the click event in derived classes
        /// </summary>
        protected virtual void OnClick() => Broadcast(ClickEvent, this);

        /// <summary>
        /// Handle button being pressed
        /// </summary>
        protected virtual void OnPress() => Broadcast(PressEvent, this);

        /// <summary>
        /// Handle button being released
        /// </summary>
        protected virtual void OnRelease() => Broadcast(ReleaseEvent, this);
    }
}
