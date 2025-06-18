/*

    Copyright (c) 2024 NoZ Games, LLC. All rights reserved.

*/

namespace NoZ.Audio
{
    internal struct AudioRandomRange
    {
        public static readonly AudioRandomRange One = new AudioRandomRange { Min = 1, Max = 1 };

        public float Min;
        public float Max;

        public readonly float GetRandomValue() => Randomizer.Range(Min, Max);
    }

    // SDL3 stub for AudioShader
    public class AudioShader : Resource<AudioShader>
    {
        public AudioShader() { }
        public float GetPitch() => 1.0f;
        public float GetVolume() => 1.0f;
        protected internal override void Unload() { }
    }
}
