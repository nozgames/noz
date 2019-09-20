using System;
using System.IO;

//using NoZ.Serialization;

namespace NoZ {
//    [SharedResource]
//    [Version(1)]
//    [SerializedType(Allocator = typeof(AudioClipAllocator))]
    public abstract class AudioClip : Resource { // : IResource, ISerializedType {
        public int SampleCount { get; private set; } = 0;
        public int Frequency { get; private set; } = 44100;
        public AudioChannelFormat ChannelFormat { get; private set; } = AudioChannelFormat.Stereo;

        /// <summary>
        /// Name of the resource that was loaded to create the audio clip.
        /// </summary>
//        Resource IResource.Resource { get; set; }

        public static AudioClip Create(string name, BinaryReader reader) {
            var clip = Game.AudioDriver.CreateClip();
            return null;
        }

        public static AudioClip Create(int samples, AudioChannelFormat channelFormat, int frequency) {
#if false
            return Game.AudioDriver.CreateClip(samples, channelFormat, frequency);
#else
            return null;
#endif
        }

        protected AudioClip() : base(null) {
        }

        protected AudioClip(int samples, AudioChannelFormat channelFormat, int frequency) : base (null) {
            SampleCount = samples;
            ChannelFormat = channelFormat;
            Frequency = frequency;
        }

        public abstract void SetData(short[] data, int offset);
        public abstract void GetData(short[] data, int offset);

        public void SetData(float[] data, int offset) {
            throw new NotImplementedException();
        }

        public void GetData(float[] data, int offset) {
            throw new NotImplementedException();
        }

        public void Play() {
            Audio.Play(this);
        }

        void Deserialize(BinaryReader reader)
        {
            ChannelFormat = (AudioChannelFormat)reader.ReadByte();
            Frequency = reader.ReadInt32();
            SampleCount = reader.ReadInt32();
            if (SampleCount == 0)
                return;

            SetData(reader.ReadShorts(SampleCount), 0);
        }

        void Serialize(BinaryWriter writer)
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

#if false
        private static class AudioClipAllocator {
            public static object CreateInstance() => Game.AudioDriver.CreateClip();
        }
#endif
    }
}
