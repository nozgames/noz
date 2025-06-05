/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

using System.Net;
using System.Net.Sockets;

namespace NoZ.Networking
{
    public class NetworkClient : IDisposable
    {
        private readonly System.Net.Sockets.Socket _socket;
        private readonly byte[] _receiveBuffer;
        private const int BufferSize = 65536;
        private Task? _receiveTask;
        private bool _running;
        public uint PlayerId { get; private set; }
        public IPEndPoint? ServerEndPoint { get; private set; }
        public bool IsConnected { get; private set; }

        /// <summary>
        /// Initializes a new instance of the NetworkClient class.
        /// </summary>
        public NetworkClient()
        {
            _socket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _receiveBuffer = new byte[BufferSize];
        }

        /// <summary>
        /// Connects to the specified server endpoint and starts the receive loop.
        /// </summary>
        /// <param name="host">The server host address.</param>
        /// <param name="port">The server port.</param>
        public void Connect(string host, int port)
        {
            Console.WriteLine($"[CLIENT] Connecting to server {host}:{port}...");
            ServerEndPoint = new IPEndPoint(Dns.GetHostAddresses(host)[0], port);
            _running = true;
            _receiveTask = Task.Run(ReceiveLoopAsync);
            SendHandshakeRequest();
        }

        /// <summary>
        /// Sends a handshake request to the server to initiate connection.
        /// </summary>
        private unsafe void SendHandshakeRequest()
        {
            var msg = stackalloc byte[1];
            msg[0] = (byte)MessageType.Connect;
            var msgSpan = new ReadOnlySpan<byte>(msg, 1);
            _socket.SendTo(msgSpan, SocketFlags.None, ServerEndPoint!);
        }

        /// <summary>
        /// Sends a custom message of type T to the server with a messageId.
        /// </summary>
        /// <typeparam name="T">The unmanaged type of the message payload.</typeparam>
        /// <param name="messageId">The custom message id.</param>
        /// <param name="value">The message payload.</param>
        public unsafe void Send<T>(byte messageId, in T value) where T : unmanaged
        {
            if (ServerEndPoint == null)
                return;
            var buffer = stackalloc byte[1 + sizeof(T)];
            buffer[0] = (byte)(MessageType.Custom + messageId);

            var bufferSpan = new Span<byte>(buffer, sizeof(T) + 1);
            fixed (byte* dst = bufferSpan.Slice(1))
                *(T*)dst = value;
            _socket.SendTo(bufferSpan, SocketFlags.None, ServerEndPoint);
        }

        /// <summary>
        /// Asynchronous receive loop for handling incoming messages from the server.
        /// </summary>
        private async Task ReceiveLoopAsync()
        {
            var remoteEP = new IPEndPoint(IPAddress.Any, 0);
            while (_running)
            {
                try
                {
                    var segment = new ArraySegment<byte>(_receiveBuffer);
                    var result = await System.Net.Sockets.SocketTaskExtensions.ReceiveFromAsync(
                        _socket, segment, SocketFlags.None, remoteEP);
                    if (ServerEndPoint != null && !result.RemoteEndPoint.Equals(ServerEndPoint))
                        continue;

                    var data = new byte[result.ReceivedBytes];
                    Array.Copy(_receiveBuffer, 0, data, 0, result.ReceivedBytes);

                    if (data.Length > 0 && data[0] == (byte)MessageType.Connect && data.Length >= 5)
                    {
                        // Handshake response: [1 byte: type=Connect][4 bytes: playerId]
                        PlayerId = BitConverter.ToUInt32(data, 1);
                        IsConnected = true;
                        OnConnected(PlayerId);
                    }
                    else
                    {
                        OnDataReceived(data);
                    }
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch (SocketException ex)
                {
                    if (!_running)
                        break; // Suppress on shutdown
                    // Optionally log: Console.WriteLine($"[CLIENT] SocketException: {ex.Message}");
                }
                catch
                {
                    // Log or handle error
                }
            }
        }

        /// <summary>
        /// Called when the client successfully connects and receives a player ID from the server.
        /// </summary>
        /// <param name="playerId">The assigned player ID.</param>
        protected virtual void OnConnected(uint playerId)
        {
            Console.WriteLine($"[CLIENT] Connected to server with PlayerId: {playerId}");
        }

        /// <summary>
        /// Called when data is received from the server.
        /// </summary>
        /// <param name="data">The received data.</param>
        protected virtual void OnDataReceived(ReadOnlySpan<byte> data)
        {
            // Override or subscribe to handle other data
        }

        /// <summary>
        /// Sends data to the server.
        /// </summary>
        /// <param name="data">The data to send.</param>
        public virtual void Send(ReadOnlySpan<byte> data)
        {
            if (ServerEndPoint != null)
                _socket.SendTo(data.ToArray(), SocketFlags.None, ServerEndPoint);
        }

        /// <summary>
        /// Disposes the client and closes the socket.
        /// </summary>
        public void Dispose()
        {
            _running = false;
            _socket.Close();
        }
    }
}
