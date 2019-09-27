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
    public enum InputAxis
    {
        MouseWheel
    }

    public enum TouchPhase
    {
        Began,
        Moved,
        Ended,
        Cancelled
    };

    public class Touch
    {
        /// Unique identifier of the touch
        public ulong Id { get; internal set; }

        /// Screen position of the touch
        public Vector2 Position { get; internal set; }

        /// Delta position from last frame.
        public Vector2 Delta { get; internal set; }

        /// Tap count
        public int TapCount { get; internal set; }

        /// Touch phase.
        public TouchPhase Phase { get; internal set; }

        public float Timestamp { get; internal set; }
    };

    public static class Input
    {
        public static readonly Event<KeyCode> KeyUpEvent = new Event<KeyCode>();
        public static readonly Event<KeyCode> KeyDownEvent = new Event<KeyCode>();

        private const int MaxTouchCount = 8;

        [Flags]
        private enum ButtonState : byte
        {
            None = 0,
            Down = 1,
            Pressed = 2,
            Released = 4,
            Drag = 8
        }

        private static bool HasAllFlags(this ref ButtonState value, ButtonState flags) => (value & flags) == flags;
        private static bool HasAnyFlags(this ref ButtonState value, ButtonState flags) => (value & flags) != 0;
        private static void SetFlags(this ref ButtonState value, ButtonState flags) => value = value | flags;
        private static void ClearFlags(this ref ButtonState value, ButtonState flags) => value = (value & ~flags);

        internal class InputEventDelegate : Object
        {
            public void OnKeyDown(KeyCode key) => Input.OnKeyDown(key);
            public void OnKeyUp(KeyCode key) => Input.OnKeyUp(key);
            public void OnMouseButtonDown(MouseButton button) => Input.OnMouseButtonDown(button);
            public void OnMouseButtonUp(MouseButton button) => Input.OnMouseButtonUp(button);
            public void OnMouseMove(Vector2 pos) => MousePosition = pos;
        }

        internal struct KeyEvent
        {
            public bool IsDown;
            public KeyCode KeyCode;
            public bool Shift;
            public bool Control;
            public bool Alt;
        }

        private struct MouseButtonInfo
        {
            public ButtonState state;
            public Vector2 dragOrigin;
        }

        /// <summary>
        /// Current state of the mouse buttons
        /// </summary>
        private static MouseButtonInfo[] _mouseButtons;

        /// <summary>
        /// Current position of the mouse
        /// </summary>
        private static Vector2 _mousePosition;

        /// <summary>
        /// Position of the mouse on the last game frame
        /// </summary>
        private static Vector2 _mousePositionPreviousFrame;

        private static float[] _axis;

        private static ButtonState[] _keys;

        private static Touch[] _touches;

        private static int _touchCount;

        private static ulong _mouseTouchId = 0;

        public static float MinimumDragDistance = 2;

        public static event Action<Node, Node> MouseOverChanged = delegate { };

        internal static List<KeyEvent> KeyEvents { get; private set; }

        private static InputEventDelegate _eventDelegate;

        public static Vector2 MousePosition {
            get {
                return _mousePosition;
            }
            internal set {
                _mousePosition = value;
                MouseDelta = _mousePosition - _mousePositionPreviousFrame;

                for (int i = 0; i < _mouseButtons.Length; i++)
                {
                    ref var info = ref _mouseButtons[i];
                    if (info.state.HasAllFlags(ButtonState.Down) && !info.state.HasAllFlags(ButtonState.Drag))
                    {
                        if ((MousePosition - info.dragOrigin).MagnitudeSquared >= MinimumDragDistance * MinimumDragDistance)
                            info.state.SetFlags(ButtonState.Drag);
                    }
                }
            }
        }

        /// <summary>
        /// Current top most node the mouse is over
        /// </summary>
        public static Node MouseOver { get; private set; }

        /// <summary>
        /// Delta mouse movement this frame
        /// </summary>
        public static Vector2 MouseDelta { get; private set; }


        internal static void Initialize(Window window)
        {
            // Already initialized?
            if (_keys != null)
                return;

            _touchCount = 0;
            _touches = new Touch[MaxTouchCount];
            for (int i = 0; i < MaxTouchCount; i++)
                _touches[i] = new Touch();

            _keys = new ButtonState[255];

            KeyEvents = new List<KeyEvent>(32);

            // Allocate the input axis array
            _axis = new float[Enum.GetValues(typeof(InputAxis)).Length];

            // Initialize the mouse button states
            _mouseButtons = new MouseButtonInfo[3];
            for (int i = 0; i < 3; i++)
                _mouseButtons[i].state = ButtonState.None;

            _mousePosition = Vector2.Zero;
            _mousePositionPreviousFrame = MousePosition;
            MouseDelta = Vector2.Zero;

            // Subscript to the window events
            _eventDelegate = new InputEventDelegate();
            Window.MouseButtonDownEvent.Subscribe(_eventDelegate.OnMouseButtonDown);
            Window.MouseButtonUpEvent.Subscribe(_eventDelegate.OnMouseButtonUp);
            Window.KeyDownEvent.Subscribe(_eventDelegate.OnKeyDown);
            Window.KeyUpEvent.Subscribe(_eventDelegate.OnKeyUp);
            Window.MouseMoveEvent.Subscribe(_eventDelegate.OnMouseMove);
#if false
            window.Subscribe(GameWindow.TouchBegan, OnTouchBegan);
            window.Subscribe(GameWindow.TouchEnded, OnTouchEnded);
            window.Subscribe(GameWindow.TouchCancelled, OnTouchCancelled);
            window.Subscribe(GameWindow.TouchMoved, OnTouchMoved);
#endif
        }

        /// <summary>
        /// Handle the ending of a frame 
        /// </summary>
        internal static void EndFrame()
        {
            // Now that the frame is over we can update the button states 
            // to account for all the button actions that happened in the frame.
            for (int i = 0; i < _mouseButtons.Length; i++)
            {
                ref var button = ref _mouseButtons[i];
                if (button.state.HasAllFlags(ButtonState.Released))
                {
                    button.state = ButtonState.None;
                }
                else if (button.state.HasAllFlags(ButtonState.Pressed))
                {
                    button.state.ClearFlags(ButtonState.Pressed);
                }
            }

            for (int i = 0; i < 255; i++)
            {
                ref var key = ref _keys[i];
                if (key.HasAllFlags(ButtonState.Released))
                {
                    key = ButtonState.None;
                }
                else if (key.HasAllFlags(ButtonState.Pressed))
                {
                    key.ClearFlags(ButtonState.Pressed);
                }
            }

            KeyEvents.Clear();

            // Save the mouse position from this frame for calculating
            // the mouse delta on the next frame
            _mousePositionPreviousFrame = MousePosition;

            for (int i = 0; i < _axis.Length; i++)
                _axis[i] = 0.0f;

            // Remove all touches that ended last frame.
            for (int i = 0; i < _touchCount;)
            {
                if (_touches[i].Phase == TouchPhase.Ended || _touches[i].Phase == TouchPhase.Cancelled)
                {
                    if (i + 1 < _touchCount)
                    {
                        var temp = _touches[i];
                        _touches[i] = _touches[_touchCount - 1];
                        _touches[_touchCount - 1] = temp;
                    }

                    _touchCount--;
                }
                else
                {
                    i++;
                }
            }

            MouseDelta = Vector2.Zero;
        }

        private static void OnKeyDown(KeyCode key)
        {
            if (_keys[(int)key].HasAllFlags(ButtonState.Pressed))
                return;
            _keys[(int)key].SetFlags(ButtonState.Down | ButtonState.Pressed);
            KeyEvents.Add(new KeyEvent
            {
                IsDown = true,
                KeyCode = key,
                Shift = IsKeyDown(KeyCode.Shift),
                Control = IsKeyDown(KeyCode.Control),
                Alt = IsKeyDown(KeyCode.Alt)
            });
        }

        private static void OnKeyUp(KeyCode key)
        {
            if (!IsKeyDown(key))
                return;

            _keys[(int)key].SetFlags(ButtonState.Released);
            KeyEvents.Add(new KeyEvent
            {
                IsDown = false,
                KeyCode = key,
                Shift = IsKeyDown(KeyCode.Shift),
                Control = IsKeyDown(KeyCode.Control),
                Alt = IsKeyDown(KeyCode.Alt)
            });
        }

        internal static void SetAxis(InputAxis axis, float value) => _axis[(int)axis] = value;

        public static float GetAxis(InputAxis axis) => _axis[(int)axis];

        public static bool IsKeyDown(KeyCode key) => _keys[(int)key].HasAllFlags(ButtonState.Down);

        public static bool WasKeyPressed(KeyCode key) => _keys[(int)key].HasAllFlags(ButtonState.Pressed);

        public static bool WasKeyReleased(KeyCode key) => _keys[(int)key].HasAllFlags(ButtonState.Released);

        /// <summary>
        /// Returns true if the given mouse button is currently held down.
        /// </summary>
        /// <param name="button">Button to check</param>
        /// <returns>True if the button is being held down</returns>
        public static bool IsButtonDown(MouseButton button)
        {
            return GetMouseButton(button).state.HasAllFlags(ButtonState.Down);
        }

        /// <summary>
        /// Was the given mouse button pressed during the current frame. Note it is 
        /// possible for a button to be pressed and released in the same frame.
        /// </summary>
        /// <param name="button">Mouse button to check.</param>
        /// <returns>True if the given mouse button was pressed during the current frame.</returns>
        public static bool WasButtonPressed(MouseButton button)
        {
            return GetMouseButton(button).state.HasAllFlags(ButtonState.Pressed);
        }

        public static bool WasButtonReleased(MouseButton button)
        {
            return GetMouseButton(button).state.HasAllFlags(ButtonState.Released);
        }

        private static ref MouseButtonInfo GetMouseButton(MouseButton button)
        {
            return ref _mouseButtons[(int)button];
        }

        public static Vector2 GetMouseButtonDrag(MouseButton button)
        {
            ref var info = ref GetMouseButton(button);
            if (!info.state.HasAllFlags(ButtonState.Drag))
                return Vector2.Zero;

            return MousePosition - info.dragOrigin;
        }

        /// <summary>
        /// Begin an input frame
        /// </summary>
        internal static void BeginFrame()
        {
            // Update the mouse over node
            var oldMouseOver = MouseOver;
            MouseOver = Node.UpdateMouseOvers();
            if (!ReferenceEquals(oldMouseOver, MouseOver))
                MouseOverChanged(oldMouseOver, MouseOver);

            // Broadcast key events
            foreach (var ke in KeyEvents)
            {
                if (ke.IsDown)
                    KeyDownEvent.Broadcast(ke.KeyCode);
                else
                    KeyUpEvent.Broadcast(ke.KeyCode);
            }
        }


        public static char KeyCodeToChar(KeyCode code, bool shift = false)
        {
            // Letters
            if (code >= KeyCode.A && code <= KeyCode.Z)
            {
                if (shift)
                {
                    return (char)('A' + (code - KeyCode.A));
                }
                return (char)('a' + (code - KeyCode.A));

                // Numbers
            }
            else if (code >= KeyCode.D0 && code <= KeyCode.D9)
            {
                if (shift)
                {
                    switch (code)
                    {
                        case KeyCode.D0:
                            return ')';
                        case KeyCode.D1:
                            return '!';
                        case KeyCode.D2:
                            return '@';
                        case KeyCode.D3:
                            return '#';
                        case KeyCode.D4:
                            return '$';
                        case KeyCode.D5:
                            return '%';
                        case KeyCode.D6:
                            return '^';
                        case KeyCode.D7:
                            return '&';
                        case KeyCode.D8:
                            return '*';
                        case KeyCode.D9:
                            return '(';
                        default:
                            break;
                    }
                }

                return (char)('0' + (code - KeyCode.D0));
            }

            switch (code)
            {
                case KeyCode.Space:
                    return ' ';
                case KeyCode.Comma:
                    return ',';
                case KeyCode.Period:
                    return '.';
                case KeyCode.Minus:
                    return '-';
                case KeyCode.Semicolon:
                    return shift ? ':' : ';';
            }

            return '\0';
        }

        private static int FindTouch(ulong id)
        {
            for (int i = 0; i < _touchCount; i++)
                if (_touches[i].Id == id)
                    return i;

            return -1;
        }

        private static void UpdateTouch(Touch touch, TouchPhase phase, in Vector2 position, int tapCount)
        {
            touch.Position = position;
            touch.TapCount = tapCount;
            touch.Phase = phase;
            touch.Timestamp = Time.TotalTime;

            // If the touch is the touch simulating the mouse then update the mouse position
            if (touch.Id == _mouseTouchId)
                MousePosition = position;
        }

        private static void OnMouseButtonDown(MouseButton button)
        {
            ref var info = ref GetMouseButton(button);
            if (info.state.HasAllFlags(ButtonState.Pressed))
                return;
            info.state.SetFlags(ButtonState.Down | ButtonState.Pressed);
            info.dragOrigin = MousePosition;
        }

        private static void OnMouseButtonUp(MouseButton button)
        {
            if (IsButtonDown(button))
                GetMouseButton(button).state.SetFlags(ButtonState.Released);
        }

        private static void OnTouchBegan(ulong touchId, Vector2 position, int tapCount)
        {
            if (_touchCount >= MaxTouchCount)
                return;

            // Simulate mouse events using the first touch
            if (_mouseTouchId == 0)
                _mouseTouchId = touchId;

            var touch = _touches[_touchCount++];
            touch.Id = touchId;
            UpdateTouch(touch, TouchPhase.Began, position, tapCount);

            if (_mouseTouchId == touchId)
                OnMouseButtonDown(MouseButton.Left);
        }

        private static void OnTouchEnded(ulong touchId, Vector2 position, int tapCount)
        {
            var touchIndex = FindTouch(touchId);
            if (-1 == touchIndex)
                return;

            UpdateTouch(_touches[touchIndex], TouchPhase.Ended, position, tapCount);
            _touches[touchIndex].Id = 0;

            if (touchId == _mouseTouchId)
            {
                _mouseTouchId = 0;
                OnMouseButtonUp(MouseButton.Left);
            }
        }

        private static void OnTouchCancelled(ulong touchId, Vector2 position, int tapCount)
        {
            var touchIndex = FindTouch(touchId);
            if (-1 == touchIndex)
                return;

            UpdateTouch(_touches[touchIndex], TouchPhase.Cancelled, position, tapCount);
            _touches[touchIndex].Id = 0;

            if (touchId == _mouseTouchId)
            {
                _mouseTouchId = 0;
                OnMouseButtonUp(MouseButton.Left);
            }
        }

        private static void OnTouchMoved(ulong touchId, Vector2 position, int tapCount)
        {
            var touchIndex = FindTouch(touchId);
            if (-1 == touchIndex)
                return;

            var touch = _touches[touchIndex];
            touch.Delta = position - touch.Position;
            touch.Position = position;
            touch.Phase = TouchPhase.Moved;
            touch.Timestamp = Time.TotalTime;
            touch.TapCount = tapCount;

            // If the touch is the touch simulating the mouse then update the mouse position
            if (touchId == _mouseTouchId)
                MousePosition = position;
        }
    }
}
