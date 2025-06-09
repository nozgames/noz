/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using NoZ.Collections;

namespace NoZ.Networking
{
    public class NetworkClient : NetworkBase, IDisposable
    {
        private Task? _receiveTask;
        private bool _running;
        public uint PlayerId { get; private set; }
        public IPEndPoint? ServerEndPoint { get; private set; }
        private DateTime _lastHeartbeatSent = DateTime.MinValue;
        private double _serverTime;
        private double _rtt;
        private double _heartbeatIntervalSeconds = 1.0; // Send heartbeat every second by default
        private DateTime _clientStartTime = DateTime.UtcNow;

        public event Action<uint>? Connected;
        public event Action? Disconnected;

        /// <summary>
        /// Initializes a new instance of the NetworkClient class.
        /// </summary>
        public NetworkClient() : base() 
        {
            // Ensure the socket is bound to a local endpoint for UDP
            if (!_socket.IsBound)
                _socket.Bind(new IPEndPoint(IPAddress.Any, 0)); // Bind to any available port
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
            Console.WriteLine("[CLIENT] Connect() called, waiting for handshake response...");
        }

        /// <summary>
        /// Sends a handshake request to the server to initiate connection.
        /// </summary>
        private unsafe void SendHandshakeRequest()
        {
            var msg = stackalloc byte[1];
            msg[0] = (byte)MessageType.Connect;
            var msgSpan = new ReadOnlySpan<byte>(msg, 1);
            this._socket.SendTo(msgSpan, SocketFlags.None, ServerEndPoint!);
        }

        /// <summary>
        /// Sends a custom message of type T to the server with a messageId.
        /// </summary>
        /// <typeparam name="T">The unmanaged type of the message payload.</typeparam>
        /// <param name="messageId">The custom message id.</param>
        /// <param name="value">The message payload.</param>
        public void Send<T>(byte messageId, in T value) where T : unmanaged
        {
            if (ServerEndPoint == null)
                return;
            SendMessage(ServerEndPoint, messageId, value);
        }

        /// <summary>
        /// Call this periodically to send a heartbeat to the server.
        /// </summary>
        public unsafe void SendHeartbeat()
        {
            if (ServerEndPoint == null) return;
            var now = DateTime.UtcNow;
            var msg = new Messages.Heartbeat { ClientTicks = now.Ticks, ServerTicks = 0 };
            var buffer = stackalloc byte[1 + Messages.Heartbeat.Size];
            buffer[0] = (byte)MessageType.Heartbeat;
            *(Messages.Heartbeat*)(buffer + 1) = msg;
            this._socket.SendTo(new ReadOnlySpan<byte>(buffer, 1 + Messages.Heartbeat.Size), SocketFlags.None, ServerEndPoint);
            _lastHeartbeatSent = now;
        }

        /// <summary>
        /// Gets the last known server time (approximate, based on heartbeat RTT).
        /// </summary>
        public double ServerTime => _serverTime;

        /// <summary>
        /// Gets the last measured round-trip time (RTT) in seconds.
        /// </summary>
        public double RoundTripTime => _rtt;

        /// <summary>
        /// Sets the heartbeat interval in seconds (default: 1.0).
        /// </summary>
        public void SetHeartbeatInterval(double seconds)
        {
            _heartbeatIntervalSeconds = seconds;
        }

        /// <summary>
        /// Should be called regularly to handle heartbeat sending.
        /// </summary>
        public virtual void Update()
        {
            if (!_running)
                return;
            if (!IsConnected)
                return;
            var now = DateTime.UtcNow;
            if ((now - _lastHeartbeatSent).TotalSeconds >= _heartbeatIntervalSeconds)
                SendHeartbeat();
        }

        /// <summary>
        /// Asynchronous receive loop for handling incoming messages from the server.
        /// </summary>
        private async Task ReceiveLoopAsync()
        {
            var remoteEP = new IPEndPoint(IPAddress.Any, 0);
            var segment = new ArraySegment<byte>(_receiveBuffer);
            while (_running)
            {
                try
                {
                    var result = await System.Net.Sockets.SocketTaskExtensions.ReceiveFromAsync(
                        this._socket,
                        segment,
                        SocketFlags.None,
                        remoteEP);

                    if (ServerEndPoint != null && !result.RemoteEndPoint.Equals(ServerEndPoint))
                        continue;

                    var data = _receiveBufferSpan.Slice(0, result.ReceivedBytes);
                    if (data.Length == 0)
                        continue;

                    HandleMessage(data);

                }
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch (SocketException ex)
                {
                    if (!_running)
                        break; // Suppress on shutdown
                    Console.WriteLine($"[CLIENT] SocketException: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[CLIENT] Exception: {ex.Message}");
                }
            }
        }

        protected virtual void HandleMessage(UnsafeSpan<byte> data)
        {
            switch (data[0])
            {
                case (byte)MessageType.Connect:
                    HandleConnect(data.Slice(1));
                    break;

                case (byte)MessageType.Heartbeat:
                    HandleHeartbeat(data.Slice(1));
                    break;

                case >= (byte)MessageType.Custom:
                    HandleCustomMessage(data);
                    break;
            }
        }

        private unsafe void HandleConnect(UnsafeSpan<byte> data)
        {
            if (data.Length != Messages.Handshake.Size)
                return;

            var handshake = (Messages.Handshake*)data.Pointer;
            PlayerId = handshake->PlayerId;
            IsConnected = true;
            OnConnected(PlayerId);
        }

        private unsafe void HandleHeartbeat(UnsafeSpan<byte> data)
        {
            var heartbeat = (Messages.Heartbeat*)data.Pointer;

            var now = DateTime.UtcNow;
            var sentTime = new DateTime(heartbeat->ClientTicks);
            _rtt = (now - sentTime).TotalSeconds;
            var newServerTime = heartbeat->ServerTicks + (_rtt * 1000.0 * 0.5);
            _serverTime = Math.Max(_serverTime, newServerTime);
        }

        /// <summary>
        /// Called when the client successfully connects and receives a player ID from the server.
        /// </summary>
        /// <param name="playerId">The assigned player ID.</param>
        protected virtual void OnConnected(uint playerId)
        {
            Connected?.Invoke(playerId);
        }

        /// <summary>
        /// Called when data is received from the server.
        /// </summary>
        /// <param name="data">The received data as UnsafeSpan.</param>
        protected void HandleCustomMessage(UnsafeSpan<byte> data)
        {
            var messageId = (byte)(data[0] - (byte)MessageType.Custom);
            base.InvokeCustomHandler(messageId, null, data.Slice(1));
        }

        /// <summary>
        /// Sends data to the server.
        /// </summary>
        /// <param name="data">The data to send.</param>
        public virtual void Send(ReadOnlySpan<byte> data)
        {
            if (ServerEndPoint != null)
                this._socket.SendTo(data.ToArray(), SocketFlags.None, ServerEndPoint);
        }

        /// <summary>
        /// Disposes the client and closes the socket.
        /// </summary>
        public void Dispose()
        {
            _running = false;
            this._socket.Close();
            Disconnected?.Invoke();
        }
    }
}
