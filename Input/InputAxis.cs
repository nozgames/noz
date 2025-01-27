/*

    Copyright (c) 2024 NoZ Games, LLC. All rights reserved.

*/

using System.Numerics;

namespace NoZ.Input
{
    public struct InputAxis(InputAxisId id = InputAxisId.None)
    {
        public InputAxisId Id { get; internal set; } = id;

        public float Value { get; internal set; }
    }
}
