/*

    Copyright (c) 2024 NoZ Games, LLC. All rights reserved.

*/

using System.Numerics;

namespace NoZ.Input
{
    /// <summary>
    /// Track and render all sprites in the game each frame
    /// </summary>
    public static class InputSystem
    {
        private const float GamePadAxisButtonThreshold = 0.5f;

        public static void Initialize()
        {
        }

        public static void Shutdown()
        {
        }

        public static void Update()
        {
        }

        public static void Read(ref InputButton button)
        {
            if (button.Id == InputButtonId.None)
                return;

            if (TryGetKeyboardKey(ref button))
                return;

            if (TryGetMouseButton(button.Id, out var mouseButton))
            {
                button.IsDown = false; // Stub for SDL3 migration
                button.IsPressed = false; // Stub for SDL3 migration
                button.IsReleased = false; // Stub for SDL3 migration
                return;
            }

            if (TryGetGamepadButton(ref button))
                return;

            if (TryGetGamepadAxisButton(ref button))
                return;
        }

        public static void Read(ref InputAxis axis, float threshold=0.001f)
        {
            switch (axis.Id)
            {
                case InputAxisId.GamePadLeftX:
                    axis.Value = 0.0f; // Stub for SDL3 migration
                    break;
                
                case InputAxisId.GamePadLeftY:
                    axis.Value = 0.0f; // Stub for SDL3 migration
                    break;

                case InputAxisId.GamePadRightX:
                    axis.Value = 0.0f; // Stub for SDL3 migration
                    break;

                case InputAxisId.GamePadRightY:
                    axis.Value = 0.0f; // Stub for SDL3 migration
                    break;

                case InputAxisId.GamePad2LeftX:
                    axis.Value = 0.0f; // Stub for SDL3 migration
                    break;

                case InputAxisId.GamePad2LeftY:
                    axis.Value = 0.0f; // Stub for SDL3 migration
                    break;

                case InputAxisId.GamePad2RightX:
                    axis.Value = 0.0f; // Stub for SDL3 migration
                    break;

                case InputAxisId.GamePad2RightY:
                    axis.Value = 0.0f; // Stub for SDL3 migration
                    break;
            }

            if (axis.Value >= -threshold && axis.Value <= threshold)
                axis.Value = 0.0f;
        }

        private static bool TryGetKeyboardKey(ref InputButton button)
        {
            var key = button.Id switch
            {
                InputButtonId.A => KeyboardKey.A,
                InputButtonId.S => KeyboardKey.S,
                InputButtonId.D => KeyboardKey.D,
                InputButtonId.W => KeyboardKey.W,
                InputButtonId.F => KeyboardKey.F,
                InputButtonId.X => KeyboardKey.X,
                InputButtonId.Up => KeyboardKey.Up,
                InputButtonId.Down => KeyboardKey.Down,
                InputButtonId.Right => KeyboardKey.Right,
                InputButtonId.Left => KeyboardKey.Left,
                InputButtonId.Space => KeyboardKey.Space,
                InputButtonId.Enter => KeyboardKey.Enter,
                InputButtonId.Escape => KeyboardKey.Escape,

                _ => KeyboardKey.Null,
            };

            if (key == KeyboardKey.Null)
                return false;

            button.IsDown = false; // Stub for SDL3 migration
            button.IsPressed = false; // Stub for SDL3 migration
            button.IsReleased = false; // Stub for SDL3 migration

            return true;
        }

        private static bool TryGetMouseButton(InputButtonId id, out MouseButton button)
        {
            switch(id)
            {
                case InputButtonId.MouseLeft: 
                    button = MouseButton.Left;
                    return true;
                    
                case InputButtonId.MouseRight: 
                    button = MouseButton.Right;
                    return true;

                case InputButtonId.MouseMiddle: 
                    button = MouseButton.Middle;
                    return true;
                
                default:
                    button = default;
                    return false;
            };
        }

        private static bool TryGetGamepadButton(ref InputButton button)
        {
            var gamePadIndex = 0;
            var gamePadButton = GamepadButton.Unknown;
            switch (button.Id)
            {
                case InputButtonId.GamePadA:
                    gamePadButton = GamepadButton.RightFaceDown;
                    break;

                case InputButtonId.GamePadB:
                    gamePadButton = GamepadButton.RightFaceRight;
                    break;

                case InputButtonId.GamePadX:
                    gamePadButton = GamepadButton.RightFaceLeft;
                    break;

                case InputButtonId.GamePadY:
                    gamePadButton = GamepadButton.RightFaceUp;
                    break;

                case InputButtonId.GamePadUp:
                    gamePadButton = GamepadButton.LeftFaceUp;
                    break;

                case InputButtonId.GamePadDown:
                    gamePadButton = GamepadButton.LeftFaceDown;
                    break;

                case InputButtonId.GamePadLeft:
                    gamePadButton = GamepadButton.LeftFaceLeft;
                    break;

                case InputButtonId.GamePadRight:
                    gamePadButton = GamepadButton.LeftFaceRight;
                    break;

                case InputButtonId.GamePadStart:
                    gamePadButton = GamepadButton.MiddleRight;
                    break;


                case InputButtonId.GamePad2A:
                    gamePadIndex = 1;
                    gamePadButton = GamepadButton.RightFaceDown;
                    break;

                case InputButtonId.GamePad2B:
                    gamePadIndex = 1;
                    gamePadButton = GamepadButton.RightFaceRight;
                    break;

                case InputButtonId.GamePad2X:
                    gamePadIndex = 1;
                    gamePadButton = GamepadButton.RightFaceLeft;
                    break;

                case InputButtonId.GamePad2Y:
                    gamePadIndex = 1;
                    gamePadButton = GamepadButton.RightFaceUp;
                    break;

                case InputButtonId.GamePad2Up:
                    gamePadIndex = 1;
                    gamePadButton = GamepadButton.LeftFaceUp;
                    break;

                case InputButtonId.GamePad2Down:
                    gamePadIndex = 1;
                    gamePadButton = GamepadButton.LeftFaceDown;
                    break;

                case InputButtonId.GamePad2Left:
                    gamePadIndex = 1;
                    gamePadButton = GamepadButton.LeftFaceLeft;
                    break;

                case InputButtonId.GamePad2Right:
                    gamePadIndex = 1;
                    gamePadButton = GamepadButton.LeftFaceRight;
                    break;

                case InputButtonId.GamePad2Start:
                    gamePadIndex = 1;
                    gamePadButton = GamepadButton.MiddleRight;
                    break;

                default:
                    return false;
            };

            button.IsDown = false; // Stub for SDL3 migration
            button.IsPressed = false; // Stub for SDL3 migration
            button.IsReleased = false; // Stub for SDL3 migration
            return true;
        }

        private static bool TryGetGamepadAxisButton(ref InputButton button)
        {
            var gamePadIndex = 0;
            var gamePadAxis = GamepadAxis.LeftTrigger;
            var direction = 1.0f;

            switch (button.Id)
            {
                case InputButtonId.GamePadAxisUp:
                    gamePadIndex = 0;
                    gamePadAxis = GamepadAxis.LeftY;
                    direction = -1.0f;
                    break;
                case InputButtonId.GamePadAxisDown:
                    gamePadIndex = 0;
                    gamePadAxis = GamepadAxis.LeftY;
                    break;

                case InputButtonId.GamePadAxisLeft:
                    gamePadIndex = 0;
                    gamePadAxis = GamepadAxis.LeftX;
                    direction = -1.0f;
                    break;

                case InputButtonId.GamePadAxisRight:
                    gamePadIndex = 0;
                    gamePadAxis = GamepadAxis.LeftX;
                    break;

                case InputButtonId.GamePad2AxisUp:
                    gamePadIndex = 1;
                    gamePadAxis = GamepadAxis.LeftY;
                    direction = -1.0f;
                    break;

                case InputButtonId.GamePad2AxisDown:
                    gamePadIndex = 1;
                    gamePadAxis = GamepadAxis.LeftY;
                    break;

                case InputButtonId.GamePad2AxisLeft:
                    gamePadIndex = 1;
                    gamePadAxis = GamepadAxis.LeftX;
                    direction = -1.0f;
                    break;

                case InputButtonId.GamePad2AxisRight:
                    gamePadIndex = 1;
                    gamePadAxis = GamepadAxis.LeftX;
                    break;

                default:
                    return false;
            };

            var gamePadAxisValue = 0.0f; // Stub for SDL3 migration
            var wasDown = button.IsDown;
            var isDown = false; // Stub for SDL3 migration
            button.IsReleased = wasDown && !isDown;
            button.IsPressed = !wasDown && isDown;
            button.IsDown = isDown;

            return true;
        }
    }
}
