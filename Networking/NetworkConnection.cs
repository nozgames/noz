/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

using System.Net;

namespace NoZ.Networking
{
    public class NetworkConnection
    {
        public IPEndPoint EndPoint { get; }

        public uint PlayerId { get; set; }

        public NetworkConnection(IPEndPoint endPoint, uint playerId = 0)
        {
            EndPoint = endPoint;
            PlayerId = playerId;
        }
    }
}
