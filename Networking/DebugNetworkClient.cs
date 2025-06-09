/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

using NoZ.Collections;
using System.Runtime.CompilerServices;

namespace NoZ.Networking
{
    /// <summary>
    /// Debug client that simulates network latency for testing.
    /// </summary>
    public class DebugNetworkClient : NetworkClient
    {
        private int _minPingMs;
        private int _maxPingMs;
        private readonly Queue<(DateTime sendTime, byte[] data)> _delayedOutgoing = new();
        private readonly Queue<(DateTime sendTime, byte[] data)> _delayedIncoming = new();
        private readonly Random _random = new();

        public DebugNetworkClient(int minPingMs = 0, int maxPingMs = 0)
        {
            _minPingMs = minPingMs;
            _maxPingMs = maxPingMs > minPingMs ? maxPingMs : minPingMs;
        }

        public int MinPingMs
        {
            get => _minPingMs;
            set => _minPingMs = value;
        }
        public int MaxPingMs
        {
            get => _maxPingMs;
            set => _maxPingMs = value;
        }

        /// <summary>
        /// Should be called regularly (e.g. from main loop) to process delayed messages and send heartbeats.
        /// </summary>
        public override unsafe void Update()
        {
            base.Update(); // Ensure heartbeat logic runs
            var now = DateTime.UtcNow;
            while (_delayedOutgoing.Count > 0 && _delayedOutgoing.Peek().sendTime <= now)
            {
                var msg = _delayedOutgoing.Dequeue();
                base.Send(msg.data);
            }
            
            while (_delayedIncoming.Count > 0 && _delayedIncoming.Peek().sendTime <= now)
            {
                var msg = _delayedIncoming.Dequeue();
                fixed(byte* dataPtr = msg.data)
                {
                    base.HandleMessage(new Collections.UnsafeSpan<byte>(dataPtr, msg.data.Length));
                }
            }
        }

        public override void Send(ReadOnlySpan<byte> data)
        {
            int delay = _minPingMs;
            if (_maxPingMs > _minPingMs)
                delay = _random.Next(_minPingMs, _maxPingMs + 1);
            int halfDelay = delay / 2;
            if (halfDelay > 0)
            {
                var sendTime = DateTime.UtcNow.AddMilliseconds(halfDelay);
                var copy = new byte[data.Length];
                data.CopyTo(copy);
                _delayedOutgoing.Enqueue((sendTime, copy));
            }
            else
            {
                base.Send(data);
            }
        }

        protected override unsafe void HandleMessage(UnsafeSpan<byte> data)
        {
            int delay = _minPingMs;
            if (_maxPingMs > _minPingMs)
                delay = _random.Next(_minPingMs, _maxPingMs + 1);
            int halfDelay = delay / 2;
            if (halfDelay > 0)
            {
                var sendTime = DateTime.UtcNow.AddMilliseconds(halfDelay);
                var copy = new byte[data.Length];
                fixed(byte* copyPtr = copy)
                    Unsafe.CopyBlock(copyPtr, data.Pointer, (uint)data.Length);

                _delayedIncoming.Enqueue((sendTime, copy));
            }
            else
            {
                base.HandleMessage(data);
            }
        }
    }
}
