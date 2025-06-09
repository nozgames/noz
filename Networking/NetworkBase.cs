/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

using NoZ.Collections;
using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace NoZ.Networking
{
    public unsafe abstract class NetworkBase
    {
        protected readonly Socket _socket;
        protected readonly byte[] _receiveBuffer;
        protected readonly GCHandle _receiveBufferHandle;
        protected readonly UnsafeSpan<byte> _receiveBufferSpan;

        protected const int BufferSize = 65536;
        public bool IsConnected { get; protected set; }

        // Handler: (NetworkConnection, UnsafeSpan<byte>)
        private readonly Action<NetworkConnection, UnsafeSpan<byte>>[] _customHandlers =
            new Action<NetworkConnection, UnsafeSpan<byte>>[256];

        protected NetworkBase()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _receiveBuffer = new byte[BufferSize];
            _receiveBufferHandle = GCHandle.Alloc(_receiveBuffer, GCHandleType.Pinned);
            _receiveBufferSpan = new UnsafeSpan<byte>((byte*)_receiveBufferHandle.AddrOfPinnedObject(), BufferSize);
        }

        /// <summary>
        /// Serializes and sends a custom message of type T to the specified endpoint.
        /// </summary>
        protected unsafe void SendMessage<T>(EndPoint endpoint, byte messageId, in T value) where T : unmanaged
        {
            var buffer = stackalloc byte[1 + sizeof(T)];
            buffer[0] = (byte)(NoZ.Networking.MessageType.Custom + messageId);
            *(T*)(buffer + 1) = value;
            this._socket.SendTo(new ReadOnlySpan<byte>(buffer, 1 + sizeof(T)), SocketFlags.None, endpoint);
        }

        /// <summary>
        /// Register a handler for a custom message type (server version).
        /// </summary>
        /// <typeparam name="T">The unmanaged type of the message payload.</typeparam>
        /// <param name="messageId">The custom message id (0-255).</param>
        /// <param name="handler">The handler to call when the message is received.</param>
        public void RegisterCustomHandler<T>(byte messageId, Action<NetworkConnection, T> handler) where T : unmanaged
        {
            if (_customHandlers[messageId] != null)
                throw new InvalidOperationException($"Handler already registered for messageId {messageId}");
            _customHandlers[messageId] = (conn, uspan) =>
            {
                var value = MemoryMarshal.Read<T>(uspan.AsSpan());
                handler(conn, value);
            };
        }

        /// <summary>
        /// Register a handler for a custom message type (client version).
        /// </summary>
        /// <typeparam name="T">The unmanaged type of the message payload.</typeparam>
        /// <param name="messageId">The custom message id (0-255).</param>
        /// <param name="handler">The handler to call when the message is received.</param>
        public void RegisterCustomHandler<T>(byte messageId, Action<T> handler) where T : unmanaged
        {
            if (_customHandlers[messageId] != null)
                throw new InvalidOperationException($"Handler already registered for messageId {messageId}");
            _customHandlers[messageId] = (_, uspan) =>
            {
                var value = MemoryMarshal.Read<T>(uspan.AsSpan());
                handler(value);
            };
        }

        /// <summary>
        /// Unregister a handler for a custom message type.
        /// </summary>
        public void UnregisterCustomHandler(byte messageId)
        {
            _customHandlers[messageId] = null;
        }

        /// <summary>
        /// Always call the thunk for the messageId, passing the UnsafeSpan.
        /// </summary>
        /// <param name="messageId">The custom message id (0-255).</param>
        /// <param name="connection">The network connection associated with the message.</param>
        /// <param name="data">The raw data of the message as UnsafeSpan.</param>
        protected void InvokeCustomHandler(byte messageId, NetworkConnection connection, UnsafeSpan<byte> data)
        {
            var handler = _customHandlers[messageId];
            handler?.Invoke(connection, data);
        }
    }
}
