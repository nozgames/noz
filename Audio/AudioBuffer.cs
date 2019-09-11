using System;
using System.Collections.Generic;
using System.Text;

namespace NoZ {
    public class AudioBuffer {
        private short[] _samples;
        private int _position;
        private int _size;

        public int SamplesPerSecond {
            get; private set;
        }

        public AudioChannelFormat ChannelFormat {
            get; private set;
        }

        public bool IsFull => _size >= _samples.Length;

        public AudioBuffer(uint sampleCount, uint samplesPerSecond, AudioChannelFormat channelFormat) {
            _samples = new short[sampleCount];
            _position = 0;
            SamplesPerSecond = (int)samplesPerSecond;
            ChannelFormat = channelFormat;
        }

        public void Clear() {
            _position = 0;
            _size = 0;
        }

        public int Write(short sample) {
            if (_size >= _samples.Length)
                return 0;
            _samples[_size++] = sample;
            return 1;
        }

        public int Write(short[] samples) {
            throw new NotImplementedException();
        }

        public int Read(short[] buffer, int offset, int count) {
            count = Math.Min(Math.Min(count, buffer.Length - offset), _size-_position);
            if (count == 0)
                return 0;

            Array.Copy(_samples, _position, buffer, offset, count);
            _position += count;
            return count;
        }

    }
}
