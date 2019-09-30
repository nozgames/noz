/*
  NoZ Game Engine

  Copyright(c) 2019 NoZ Games, LLC

  Permission is hereby granted, free of charge, to any person obtaining a copy
  of this software and associated documentation files(the "Software"), to deal
  in the Software without restriction, including without limitation the rights
  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
  copies of the Software, and to permit persons to whom the Software is
  furnished to do so, subject to the following conditions :

  The above copyright notice and this permission notice shall be included in all
  copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
  SOFTWARE.
*/

using System;

namespace NoZ
{
    public class AudioBuffer
    {
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

        public AudioBuffer(uint sampleCount, uint samplesPerSecond, AudioChannelFormat channelFormat)
        {
            _samples = new short[sampleCount];
            _position = 0;
            SamplesPerSecond = (int)samplesPerSecond;
            ChannelFormat = channelFormat;
        }

        public void Clear()
        {
            _position = 0;
            _size = 0;
        }

        public int Write(short sample)
        {
            if (_size >= _samples.Length)
                return 0;
            _samples[_size++] = sample;
            return 1;
        }

        public int Write(short[] samples)
        {
            throw new NotImplementedException();
        }

        public int Read(short[] buffer, int offset, int count)
        {
            count = Math.Min(Math.Min(count, buffer.Length - offset), _size - _position);
            if (count == 0)
                return 0;

            Array.Copy(_samples, _position, buffer, offset, count);
            _position += count;
            return count;
        }
    }
}
