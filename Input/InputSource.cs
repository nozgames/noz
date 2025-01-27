/*

    Copyright (c) 2024 NoZ Games, LLC. All rights reserved.

*/

namespace NoZ.Input
{
    public enum InputSource
    {
        None,

        /// <summary>
        /// Keyboard and mouse
        /// </summary>
        Keyboard,

        /// <summary>
        /// Keyboard and mouse or gamepad 1
        /// </summary>
        KeyboardOrGamePad1,

        /// <summary>
        /// Gamepad 1 only
        /// </summary>
        GamePad1,

        /// <summary>
        /// Gamepad 2 only
        /// </summary>
        GamePad2,

        Count
    }
}
