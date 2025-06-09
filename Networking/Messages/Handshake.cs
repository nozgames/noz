/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

using System.Runtime.InteropServices;

namespace NoZ.Networking.Messages
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal unsafe struct Handshake
    {
        public uint PlayerId;

        public static readonly int Size = sizeof(Handshake);
    }
}
