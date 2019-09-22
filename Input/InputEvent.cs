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

namespace NoZ
{
    public class InputEvent
    {
        private bool _handled = false;

        public bool IsHandled 
        {
            get => _handled;
            set => _handled = _handled | value;
        }

        internal virtual void Reset () => _handled = false;
    }

    public class MouseButtonEvent : InputEvent
    {
        public bool IsDown { get; internal set; } = false;
        public MouseButton Button { get; internal set; } = MouseButton.Left;
        public Vector2 Position { get; internal set; }
    }

    public class MouseOverEvent : InputEvent
    {
        public Vector2 Position { get; internal set; } = Vector2.Zero;
        public Vector2 Delta { get; internal set; } = Vector2.Zero;

#if false
        private Cursor _cursor;
        public Cursor Cursor {
            get => _cursor;
            set {
                // The first call to setting a cursor will set the cursor all others
                // will ignore the call.
                if (_cursor == null)
                    _cursor = value;
            }
        }

        internal override void Reset () {
            base.Reset();
            _cursor = null;
        }
#endif
    }

    public class MouseWheelEvent : InputEvent {
        public float Distance { get; internal set; }
    }

    public class KeyboardEvent : InputEvent {
        public KeyCode KeyCode { get; internal set; }
        public bool Shift { get; internal set; }
        public bool Control { get; internal set; }
        public bool Alt { get; internal set; }
    }

}
