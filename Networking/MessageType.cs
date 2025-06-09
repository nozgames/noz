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

        // Heartbeat message
        Heartbeat = 2,

        // Raw message
        Custom = 3,
    }
}
