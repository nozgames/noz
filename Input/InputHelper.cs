/*

    Copyright (c) 2024 NoZ Games, LLC. All rights reserved.

*/

using System.Runtime.CompilerServices;

namespace NoZ.Input
{
    public static class InputHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InputButton Or(in InputButton a, in InputButton b)
        {
            return new InputButton
            {
                IsDown = a.IsDown || b.IsDown,
                IsPressed = a.IsPressed || b.IsPressed,
                IsReleased = a.IsReleased || b.IsReleased
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InputButton Or(in InputButton a, in InputButton b, in InputButton c)
        {
            return new InputButton
            {
                IsDown = a.IsDown || b.IsDown || c.IsDown,
                IsPressed = a.IsPressed || b.IsPressed || c.IsPressed,
                IsReleased = a.IsReleased || b.IsReleased || c.IsReleased
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InputButton Or(in InputButton a, in InputButton b, in InputButton c, in InputButton d)
        {
            return new InputButton
            {
                IsDown = a.IsDown || b.IsDown || c.IsDown || d.IsDown,
                IsPressed = a.IsPressed || b.IsPressed || c.IsPressed || d.IsPressed,
                IsReleased = a.IsReleased || b.IsReleased || c.IsReleased || d.IsReleased
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InputAxis Or(in InputAxis a, in InputButton min, in InputButton max)
        {
            return new InputAxis
            {
                Value = MathF.Min(1.0f, MathF.Max(-1.0f, a.Value + (max.IsDown ? 1 : 0) - (min.IsDown ? 1 : 0)))
            };
        }
    }
}
