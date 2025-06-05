/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

namespace NoZ.Networking
{
    internal enum MessageType : byte
    {
        Unknown = 0,

        // Initial connection message
        Connect = 1,

        // Raw message
        Custom = 2,
    }
}
