/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using NoZ.Collections;
using NoZ.Networking.Messages;

namespace NoZ.Networking
{
    public class NetworkServer : NetworkBase, NoZ.IUpdatable, IDisposable
    {
        private readonly ConcurrentDictionary<IPEndPoint, (NetworkConnection connection, DateTime lastSeen)> _connections;
        private readonly TimeSpan _timeout;
        private long _serverTimeMs = 0;
        private DateTime _lastServerTimeUpdate = DateTime.UtcNow;
        private bool _running;
        private Task? _receiveTask;
        private uint _nextPlayerId = 1;

        public int Port { get; }

        /// <summary>
        /// Returns all known connections.
        /// </summary>
        public IReadOnlyCollection<NetworkConnection> Connections =>
            _connections.Values.Select(v => v.connection).ToList();

        /// <summary>
        /// Initializes a new instance of the NetworkServer class and starts listening for connections.
        /// </summary>
        /// <param name="port">The port to listen on.</param>
        /// <param name="timeoutSeconds">The timeout in seconds for inactive connections.</param>
        public NetworkServer(int port = 0, int timeoutSeconds = 30) : base()
        {
            Port = port;
            _socket.Bind(new IPEndPoint(IPAddress.Any, Port));
            _connections = new ConcurrentDictionary<IPEndPoint, (NetworkConnection, DateTime)>();
            _timeout = TimeSpan.FromSeconds(timeoutSeconds);
            _running = true;
            _receiveTask = Task.Run(ReceiveLoopAsync);
        }

        /// <summary>
        /// Sends a custom message of type T to a client connection with a messageId.
        /// </summary>
        /// <typeparam name="T">The unmanaged type of the message payload.</typeparam>
        /// <param name="connection">The client connection to send to.</param>
        /// <param name="messageId">The custom message id.</param>
        /// <param name="value">The message payload.</param>
        public void Send<T>(NetworkConnection connection, byte messageId, in T value) where T : unmanaged
        {
            SendMessage(connection.EndPoint, messageId, value);
        }

        /// <summary>
        /// Asynchronous receive loop for handling incoming messages from clients.
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
                        _socket,
                        segment,
                        SocketFlags.None,
                        remoteEP);

                    var endpoint = (IPEndPoint)result.RemoteEndPoint;
                    var now = DateTime.UtcNow;
                    var connectionTuple = _connections.GetOrAdd(endpoint, ep => (new NetworkConnection(ep), now));
                    _connections[endpoint] = (connectionTuple.connection, now);

                    var data = new UnsafeSpan<byte>(_receiveBufferSpan, 0, result.ReceivedBytes);
                    HandleMessage(connectionTuple.connection, data);
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch
                {
                    // Log or handle error
                }
            }
        }

        /// <summary>
        /// Called when data is received from a client.
        /// </summary>
        /// <param name="connection">The client connection.</param>
        /// <param name="data">The received data.</param>
        protected virtual unsafe void HandleMessage(NetworkConnection connection, UnsafeSpan<byte> data)
        {
            if (data.Length < 1)
                return;

            switch((MessageType)data[0])
            {
                case MessageType.Connect:
                    HandleConnect(connection, data.Slice(1));
                    break;

                case MessageType.Heartbeat:
                    HandleHeartbeat(connection, data.Slice(1));
                    break;

                case >= MessageType.Custom:
                    HandleCustomMessage(connection, data);
                    break;
            }
        }

        private unsafe void HandleConnect(NetworkConnection connection, UnsafeSpan<byte> data)
        {
            // Handshake request from client
            connection.PlayerId = _nextPlayerId++;
            Console.WriteLine($"[SERVER] Client joined: {connection.EndPoint} assigned PlayerId: {connection.PlayerId}");
            var response = stackalloc byte[Messages.Handshake.Size + 1];
            var handshake = (Messages.Handshake*)(response + 1);
            response[0] = (byte)MessageType.Connect; // handshake response
            *handshake = new Handshake
            {
                PlayerId = connection.PlayerId
            };
            this._socket.SendTo(new ReadOnlySpan<byte>(response, Messages.Handshake.Size + 1), SocketFlags.None, connection.EndPoint);
        }

        private unsafe void HandleHeartbeat(NetworkConnection connection, UnsafeSpan<byte> data)
        {
            var clientHeartbeat = (Heartbeat*)data.Pointer;
            var response = stackalloc byte[1 + Heartbeat.Size];
            var responseHeartbeat = (Heartbeat*)(response + 1);
            *responseHeartbeat = new Heartbeat
            {
                ClientTicks = clientHeartbeat->ClientTicks,
                ServerTicks = _serverTimeMs
            };

            response[0] = (byte)MessageType.Heartbeat;
            this._socket.SendTo(new ReadOnlySpan<byte>(response, 1 + Heartbeat.Size), SocketFlags.None, connection.EndPoint);
        }

        private unsafe void HandleCustomMessage(NetworkConnection connection, UnsafeSpan<byte> data)
        {
            var messageId = (byte)(data[0] - (byte)MessageType.Custom);
            InvokeCustomHandler(messageId, connection, data.Slice(1));
        }

        /// <summary>
        /// Sends data to a client connection.
        /// </summary>
        /// <param name="connection">The client connection.</param>
        /// <param name="data">The data to send.</param>
        public virtual void Send(NetworkConnection connection, ReadOnlySpan<byte> data)
        {
            this._socket.SendTo(data.ToArray(), SocketFlags.None, connection.EndPoint); // ToArray() needed for sync API
        }

        /// <summary>
        /// Updates the server, checking for connection timeouts.
        /// </summary>
        public void Update()
        {
            // Advance server time
            var now = DateTime.UtcNow;
            var delta = (now - _lastServerTimeUpdate).TotalMilliseconds;
            if (delta > 0)
            {
                _serverTimeMs += (long)delta;
                _lastServerTimeUpdate = now;
            }

            // Timeout logic
            foreach (var kvp in _connections)
            {
                if (now - kvp.Value.lastSeen > _timeout)
                {
                    _connections.TryRemove(kvp.Key, out var removed);
                    OnConnectionTimeout(removed.connection);
                }
            }
        }

        /// <summary>
        /// Called when a client connection times out.
        /// </summary>
        /// <param name="connection">The client connection that timed out.</param>
        protected virtual void OnConnectionTimeout(NetworkConnection connection)
        {
            Console.WriteLine($"[SERVER] Client timed out and removed: {connection.EndPoint} PlayerId: {connection.PlayerId}");
        }

        /// <summary>
        /// Disposes the server and closes the socket.
        /// </summary>
        public void Dispose()
        {
            _running = false;
            this._socket.Close();
        }
    }
}
