/*

    Copyright (c) 2024 NoZ Games, LLC. All rights reserved.

*/

using NoZ.Input;

namespace NoZ.UI
{
    public static partial class UI
    {
        private static class InputMapKeyboard
        {
            private static InputButton _navigateUp1 = new InputButton(InputButtonId.Up);
            private static InputButton _navigateUp2 = new InputButton(InputButtonId.W);
            private static InputButton _navigateDown1 = new InputButton(InputButtonId.Down);
            private static InputButton _navigateDown2 = new InputButton(InputButtonId.S);
            private static InputButton _navigateLeft1 = new InputButton(InputButtonId.Left);
            private static InputButton _navigateLeft2 = new InputButton(InputButtonId.A);
            private static InputButton _navigateRight1 = new InputButton(InputButtonId.Right);
            private static InputButton _navigateRight2 = new InputButton(InputButtonId.D);
            private static InputButton _select1 = new InputButton(InputButtonId.Space);
            private static InputButton _select2 = new InputButton(InputButtonId.Enter);
            private static InputButton _back1 = new InputButton(InputButtonId.Escape);

            public static Navigation Update()
            {
                InputSystem.Read(ref _navigateUp1);
                InputSystem.Read(ref _navigateUp2);
                InputSystem.Read(ref _navigateDown1);
                InputSystem.Read(ref _navigateDown2);
                InputSystem.Read(ref _select1);
                InputSystem.Read(ref _select2);
                InputSystem.Read(ref _back1);
                InputSystem.Read(ref _navigateLeft1);
                InputSystem.Read(ref _navigateLeft2);
                InputSystem.Read(ref _navigateRight1);
                InputSystem.Read(ref _navigateRight2);

                var navigation = Navigation.None;
                if (InputHelper.Or(_navigateDown1, _navigateDown2).IsPressed)
                    navigation = Navigation.Down;
                else if (InputHelper.Or(_navigateUp1, _navigateUp2).IsPressed)
                    navigation = Navigation.Up;
                else if (InputHelper.Or(_navigateLeft1, _navigateLeft2).IsPressed)
                    navigation = Navigation.Left;
                else if (InputHelper.Or(_navigateRight1, _navigateRight2).IsPressed)
                    navigation = Navigation.Right;
                else if (InputHelper.Or(_navigateUp1, _navigateUp2).IsPressed)
                    navigation = Navigation.Up;
                else if (InputHelper.Or(_select1, _select2).IsPressed)
                    navigation = Navigation.Select;
                else if (_back1.IsPressed)
                    navigation = Navigation.Back;

                return navigation;
            }
        }

        private static class InputMapGamePad1
        {
            private static InputButton _navigateUp1 = new InputButton(InputButtonId.GamePadUp);
            private static InputButton _navigateUp2 = new InputButton(InputButtonId.GamePadAxisUp);
            private static InputButton _navigateDown1 = new InputButton(InputButtonId.GamePadDown);
            private static InputButton _navigateDown2 = new InputButton(InputButtonId.GamePadAxisDown);
            private static InputButton _navigateLeft1 = new InputButton(InputButtonId.GamePadLeft);
            private static InputButton _navigateLeft2 = new InputButton(InputButtonId.GamePadAxisLeft);
            private static InputButton _navigateRight1 = new InputButton(InputButtonId.GamePadRight);
            private static InputButton _navigateRight2 = new InputButton(InputButtonId.GamePadAxisRight);
            private static InputButton _select1 = new InputButton(InputButtonId.GamePadA);
            private static InputButton _back1 = new InputButton(InputButtonId.GamePadB);

            public static Navigation Update()
            {
                InputSystem.Read(ref _navigateUp1);
                InputSystem.Read(ref _navigateUp2);
                InputSystem.Read(ref _navigateDown1);
                InputSystem.Read(ref _navigateDown2);
                InputSystem.Read(ref _select1);
                InputSystem.Read(ref _back1);
                InputSystem.Read(ref _navigateLeft1);
                InputSystem.Read(ref _navigateLeft2);
                InputSystem.Read(ref _navigateRight1);
                InputSystem.Read(ref _navigateRight2);

                var navigation = Navigation.None;
                if (InputHelper.Or(_navigateDown1, _navigateDown2).IsPressed)
                    navigation = Navigation.Down;
                else if (InputHelper.Or(_navigateUp1, _navigateUp2).IsPressed)
                    navigation = Navigation.Up;
                else if (InputHelper.Or(_navigateLeft1, _navigateLeft2).IsPressed)
                    navigation = Navigation.Left;
                else if (InputHelper.Or(_navigateRight1, _navigateRight2).IsPressed)
                    navigation = Navigation.Right;
                else if (InputHelper.Or(_navigateUp1, _navigateUp2).IsPressed)
                    navigation = Navigation.Up;
                else if (_select1.IsPressed)
                    navigation = Navigation.Select;
                else if (_back1.IsPressed)
                    navigation = Navigation.Back;

                return navigation;
            }
        }

        private static class InputMapGamePad2
        {
            private static InputButton _navigateUp1 = new InputButton(InputButtonId.GamePad2Up);
            private static InputButton _navigateUp2 = new InputButton(InputButtonId.GamePad2AxisUp);
            private static InputButton _navigateDown1 = new InputButton(InputButtonId.GamePad2Down);
            private static InputButton _navigateDown2 = new InputButton(InputButtonId.GamePad2AxisDown);
            private static InputButton _navigateLeft1 = new InputButton(InputButtonId.GamePad2Left);
            private static InputButton _navigateLeft2 = new InputButton(InputButtonId.GamePad2AxisLeft);
            private static InputButton _navigateRight1 = new InputButton(InputButtonId.GamePad2Right);
            private static InputButton _navigateRight2 = new InputButton(InputButtonId.GamePad2AxisRight);
            private static InputButton _select1 = new InputButton(InputButtonId.GamePad2A);
            private static InputButton _back1 = new InputButton(InputButtonId.GamePad2B);

            public static Navigation Update()
            {
                InputSystem.Read(ref _navigateUp1);
                InputSystem.Read(ref _navigateUp2);
                InputSystem.Read(ref _navigateDown1);
                InputSystem.Read(ref _navigateDown2);
                InputSystem.Read(ref _select1);
                InputSystem.Read(ref _back1);
                InputSystem.Read(ref _navigateLeft1);
                InputSystem.Read(ref _navigateLeft2);
                InputSystem.Read(ref _navigateRight1);
                InputSystem.Read(ref _navigateRight2);

                var navigation = Navigation.None;
                if (InputHelper.Or(_navigateDown1, _navigateDown2).IsPressed)
                    navigation = Navigation.Down;
                else if (InputHelper.Or(_navigateUp1, _navigateUp2).IsPressed)
                    navigation = Navigation.Up;
                else if (InputHelper.Or(_navigateLeft1, _navigateLeft2).IsPressed)
                    navigation = Navigation.Left;
                else if (InputHelper.Or(_navigateRight1, _navigateRight2).IsPressed)
                    navigation = Navigation.Right;
                else if (InputHelper.Or(_navigateUp1, _navigateUp2).IsPressed)
                    navigation = Navigation.Up;
                else if (_select1.IsPressed)
                    navigation = Navigation.Select;
                else if (_back1.IsPressed)
                    navigation = Navigation.Back;

                return navigation;
            }
        }

        /// <summary>
        /// Primary input source for the menus, drives Navigation
        /// </summary>
        public static InputSource InputSource { get; set; } = InputSource.KeyboardOrGamePad1;

        /// <summary>
        /// Indicates the last input source that was used for menu navigation for primary navigation.  
        /// </summary>
        public static InputSource LastInputSource { get; private set; } = InputSource.Keyboard;

        /// <summary>
        /// Alternate input source for the menus, drives AltNavigation
        /// </summary>
        public static InputSource AltInputSource { get; set; } = InputSource.None;

        /// <summary>
        /// Primary navigation input
        /// </summary>
        public static Navigation Navigation { get; private set; }

        /// <summary>
        /// Alternative navigation input, useful for local co-op
        /// </summary>
        public static Navigation AltNavigation { get; private set; }

        private static void ReadInput()
        {
            var keyboard = InputMapKeyboard.Update();
            var gamepad1 = InputMapGamePad1.Update();
            var gamepad2 = InputMapGamePad2.Update();

            switch (InputSource)
            {
                case InputSource.Keyboard:
                    Navigation = keyboard;
                    break;

                case InputSource.KeyboardOrGamePad1:
                    if (keyboard != Navigation.None)
                    {
                        LastInputSource = InputSource.Keyboard;
                        Navigation = keyboard;
                    }
                    else if (gamepad1 != Navigation.None)
                    {
                        Navigation = gamepad1;
                        LastInputSource = InputSource.GamePad1;
                    }
                    else
                    {
                        Navigation = Navigation.None;
                    }
                    break;

                case InputSource.GamePad1:
                    Navigation = gamepad1;
                    LastInputSource = InputSource.GamePad1;
                    break;

                case InputSource.GamePad2:
                    Navigation = gamepad2;
                    LastInputSource = InputSource.GamePad2;
                    break;

                case InputSource.None:
                    Navigation = Navigation.None;
                    LastInputSource = InputSource.None;
                    break;
            }

            switch (AltInputSource)
            {
                case InputSource.Keyboard:
                    AltNavigation = keyboard;
                    break;

                case InputSource.KeyboardOrGamePad1:
                    AltNavigation = keyboard == Navigation.None ? gamepad1 : keyboard;
                    break;

                case InputSource.GamePad1:
                    AltNavigation = gamepad1;
                    break;

                case InputSource.GamePad2:
                    AltNavigation = gamepad2;
                    break;

                default:
                    AltNavigation = Navigation.None;
                    break;
            }

        }

        public static void ClearInput()
        {
            Navigation = Navigation.None;
            AltNavigation = Navigation.None;
        }
    }
}
