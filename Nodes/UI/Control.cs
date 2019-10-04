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
    public class Control : Layout
    {
        public static readonly Event<Control> MouseEnterEvent = new Event<Control>();
        public static readonly Event<Control> MouseLeaveEvent = new Event<Control>();

        /// <summary>
        /// Current control with capture
        /// </summary>
        public static Control _capture = null;

        /// <summary>
        /// True if the control has capture
        /// </summary>
        public bool HasCapture => ReferenceEquals(_capture, this);

        /// <summary>
        /// Cursor to display when the mouse is over this control
        /// </summary>
        public Node Cursor { get; set; }

        /// <summary>
        /// After called all input events will be sent directly to the node until
        /// a subsequent ReleaseCapture is called.  Note that it is possible for capture
        /// to be interrupted and if it is the OnReleaseCapture method will be called
        /// with a value of true indicating it was cancelled.  If capture is called on a node
        /// when another node already has capture the old capture will be cancelled.
        /// </summary>
        protected void Capture()
        {
            if (_capture != null)
            {
                if (ReferenceEquals(_capture, this))
                    return;

                CancelCapture();
            }

            _capture = this;
        }

        /// <summary>
        /// Release input capture for the given node.  This method must be called internally
        /// to the node and the node must have capture.  If an external source wants to
        /// release capture the CancelCapture method should be used instead.
        /// </summary>
        protected void ReleaseCapture()
        {
            if (!ReferenceEquals(_capture, this))
                return;

            _capture.OnReleaseCapture(false);
            _capture = null;
        }

        /// <summary>
        /// Returns the current capture node or null if there is none
        /// </summary>
        /// <returns></returns>
        public static Node GetCapture()
        {
            return _capture;
        }

        /// <summary>
        /// Cancel any active mouse capture
        /// </summary>
        public static void CancelCapture()
        {
            if (_capture == null)
                return;

            _capture.OnReleaseCapture(true);
            _capture = null;
        }

        /// <summary>
        /// Called when a node releases its capture
        /// </summary>
        /// <param name="cancelled">True if the capture was released by CancelCapture</param>
        protected virtual void OnReleaseCapture(bool cancelled) { }

        protected override void OnMouseEnter() => Broadcast(MouseEnterEvent, this);

        protected override void OnMouseLeave() => Broadcast(MouseLeaveEvent, this);

        protected internal override void OnMouseOver(MouseOverEvent e)
        {
            base.OnMouseOver(e);
            e.Cursor = Cursor;
        }
    }
}
