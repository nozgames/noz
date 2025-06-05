/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

using NoZ.Collections;
using System.Diagnostics;
using System.Net.Sockets;

#if false
namespace NoZ.Networking
{
    internal unsafe class Socket
    {
        private System.Net.Sockets.Socket? _socket;

        public void Receive<T>(in UnsafeSpan<T> data) where T : unmanaged
        {
            Debug.Assert(_socket != null, "Socket is not connected.");

            var span = new Span<byte>((byte*)data.Pointer, data.Length * sizeof(T));
            _socket.ReceiveFrom(span, SocketFlags.None, new System.Net.SocketAddress(AddressFamily.Unix, 0));

            var flags = SocketFlags.None;
            _socket.Receive(span, flags, out var bytesReceived);
        }

        public void Send<T>(in UnsafeSpan<T> data) where T : unmanaged
        {
            Debug.Assert(_socket != null, "Socket is not connected.");
        }

        public void Connect(string address, int port)
        {
            if (_socket != null)
                throw new InvalidOperationException("Socket is already connected.");

            _socket = new System.Net.Sockets.Socket(AddressFamily.Unix, SocketType.Dgram, ProtocolType.Udp);
            _socket.Connect(address, port);
        }

        public void Disconnect()
        {
            if (_socket == null)
                return;

            _socket.Disconnect(false);
            _socket.Close();
            _socket = null;
        }
    }
}
#endif