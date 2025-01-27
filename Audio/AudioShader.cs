/*

    Copyright (c) 2024 NoZ Games, LLC. All rights reserved.

*/

using Raylib_cs;

namespace NoZ.Audio
{
    internal struct AudioRandomRange
    {
        public static readonly AudioRandomRange One = new AudioRandomRange { Min = 1, Max = 1 };

        public float Min;
        public float Max;

        public readonly float GetRandomValue() => Randomizer.Range(Min, Max);
    }

    public class AudioShader : Resource<AudioShader>
    {
        private Sound[] _sounds;
        private AudioRandomRange _pitch;
        private AudioRandomRange _volume;

        public Sound Sound => _sounds[0];

        public Sound RandomSound => _sounds[Randomizer.Range(0, _sounds.Length)];

        internal AudioShader(Sound sound)
        {
            _pitch = AudioRandomRange.One;
            _volume = AudioRandomRange.One;
            _sounds = [sound];
        }

        internal AudioShader(string sound, AudioRandomRange? volume = default, AudioRandomRange? pitch = default)
        {
            _sounds = new Sound[] { Raylib.LoadSound(ResourceDatabase.GetFullResourcePath(sound + ".wav")) };
            _pitch = pitch ?? AudioRandomRange.One;
            _volume = volume ?? AudioRandomRange.One;
        }

        internal AudioShader(string[] sounds, AudioRandomRange? volume = default, AudioRandomRange? pitch = default)
        {
            _sounds = sounds.Select(s => Raylib.LoadSound(ResourceDatabase.GetFullResourcePath(s + ".wav"))).ToArray();
            _pitch = pitch ?? AudioRandomRange.One;
            _volume = volume ?? AudioRandomRange.One;
        }

        public float GetPitch() => _pitch.GetRandomValue();
        public float GetVolume() => _volume.GetRandomValue();

        protected internal override void Unload()
        {
        }
    }
}
