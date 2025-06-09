/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

using System.Runtime.InteropServices;

namespace NoZ.Networking.Messages
{
    /// <summary>
    /// Heartbeat message for client-server time sync and connection keepalive.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Heartbeat
    {
        public long ClientTicks; // UTC ticks when client sent heartbeat
        public long ServerTicks; // UTC ticks when server replied (0 for client->server, set for server->client)

        public static readonly int Size = sizeof(Heartbeat);
    }
}
