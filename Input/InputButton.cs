/*

    Copyright (c) 2024 NoZ Games, LLC. All rights reserved.

*/

namespace NoZ.Input
{
    public struct InputButton(InputButtonId id)
    {
        public InputButtonId Id = id;

        public bool IsPressed { get; internal set; }
        public bool IsReleased { get; internal set; }
        public bool IsDown { get; internal set; }
    }
}