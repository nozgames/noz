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
using System.IO;

namespace NoZ
{
    public abstract class AudioClip : Resource
    {
        public int SampleCount { get; private set; } = 0;
        public int Frequency { get; private set; } = 44100;
        public AudioChannelFormat ChannelFormat { get; private set; } = AudioChannelFormat.Stereo;

        public static AudioClip Create(int samples, AudioChannelFormat channelFormat, int frequency)
        {
            return Audio.Driver.CreateClip(samples, channelFormat, frequency);
        }

        protected AudioClip() : base(null)
        {
        }

        protected AudioClip(int samples, AudioChannelFormat channelFormat, int frequency) : base(null)
        {
            SampleCount = samples;
            ChannelFormat = channelFormat;
            Frequency = frequency;
        }

        public abstract void SetData(short[] data, int offset);
        public abstract void GetData(short[] data, int offset);

        public void SetData(float[] data, int offset)
        {
            throw new NotImplementedException();
        }

        public void GetData(float[] data, int offset)
        {
            throw new NotImplementedException();
        }

        public void Play()
        {
            Audio.Play(this);
        }

        public static AudioClip Create(string name, BinaryReader reader)
        {
            var clip = Audio.Driver.CreateClip();
            clip.ChannelFormat = (AudioChannelFormat)reader.ReadByte();
            clip.Frequency = reader.ReadInt32();
            clip.SampleCount = reader.ReadInt32();
            if (clip.SampleCount == 0)
                return clip;

            clip.SetData(reader.ReadShorts(clip.SampleCount), 0);

            return clip;
        }

        /// <summary>
        /// Save the image to the given stream
        /// </summary>
        /// <param name="writer">Stream to save image to</param>
        public void Save(BinaryWriter writer)
        {
            writer.Write((byte)ChannelFormat);
            writer.Write(Frequency);
            writer.Write(SampleCount);

            if (SampleCount == 0)
                return;

            short[] data = new short[SampleCount];
            GetData(data, 0);
            writer.Write(data);
        }
    }
}
