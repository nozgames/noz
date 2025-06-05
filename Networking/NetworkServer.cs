/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace NoZ.Networking
{
    public class NetworkServer : NoZ.IUpdatable, IDisposable
    {
        private readonly System.Net.Sockets.Socket _socket;
        private readonly ConcurrentDictionary<IPEndPoint, (NetworkConnection connection, DateTime lastSeen)> _connections;
        private readonly TimeSpan _timeout;
        private bool _running;
        private Task? _receiveTask;
        private readonly byte[] _receiveBuffer;
        private const int BufferSize = 65536; // Max UDP packet size
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
        public NetworkServer(int port = 0, int timeoutSeconds = 30)
        {
            Port = port;
            _socket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _socket.Bind(new IPEndPoint(IPAddress.Any, Port));
            _connections = new ConcurrentDictionary<IPEndPoint, (NetworkConnection, DateTime)>();
            _timeout = TimeSpan.FromSeconds(timeoutSeconds);
            _receiveBuffer = new byte[BufferSize];
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
        public unsafe void Send<T>(NetworkConnection connection, byte messageId, in T value) where T : unmanaged
        {
            var buffer = stackalloc byte[1 + sizeof(T)];
            buffer[0] = (byte)(MessageType.Custom + messageId);
            *(T*)(buffer + 1) = value;
            _socket.SendTo(new ReadOnlySpan<byte>(buffer, 1 + sizeof(T)), SocketFlags.None, connection.EndPoint);
        }

        /// <summary>
        /// Asynchronous receive loop for handling incoming messages from clients.
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
                        _socket,
                        segment,
                        SocketFlags.None,
                        remoteEP);

                    var endpoint = (IPEndPoint)result.RemoteEndPoint;
                    var now = DateTime.UtcNow;
                    var connectionTuple = _connections.GetOrAdd(endpoint, ep => (new NetworkConnection(ep), now));
                    _connections[endpoint] = (connectionTuple.connection, now);
                    OnDataReceived(connectionTuple.connection, _receiveBuffer.AsSpan(0, result.ReceivedBytes));
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
        protected virtual unsafe void OnDataReceived(NetworkConnection connection, ReadOnlySpan<byte> data)
        {
            if (data.Length > 0 && data[0] == (byte)MessageType.Connect)
            {
                // Handshake request from client
                connection.PlayerId = _nextPlayerId++;
                Console.WriteLine($"[SERVER] Client joined: {connection.EndPoint} assigned PlayerId: {connection.PlayerId}");
                var response = stackalloc byte[5];
                response[0] = (byte)MessageType.Connect; // handshake response
                BitConverter.TryWriteBytes(new Span<byte>(response + 1, 4), connection.PlayerId);
                _socket.SendTo(new ReadOnlySpan<byte>(response, 5), SocketFlags.None, connection.EndPoint);
            }
            else
            {
                // Handle other data
            }
        }

        /// <summary>
        /// Sends data to a client connection.
        /// </summary>
        /// <param name="connection">The client connection.</param>
        /// <param name="data">The data to send.</param>
        public virtual void Send(NetworkConnection connection, ReadOnlySpan<byte> data)
        {
            _socket.SendTo(data.ToArray(), SocketFlags.None, connection.EndPoint); // ToArray() needed for sync API
        }

        /// <summary>
        /// Updates the server, checking for connection timeouts.
        /// </summary>
        public void Update()
        {
            var now = DateTime.UtcNow;
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
            _socket.Close();
        }
    }
}
