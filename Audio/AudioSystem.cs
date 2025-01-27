/*

    Copyright (c) 2024 NoZ Games, LLC. All rights reserved.

*/

using Raylib_cs;

namespace NoZ.Audio
{
    public static class AudioSystem
    {
        public static void Play(AudioShader? shader, float volume = 1.0f, float pitch = 1.0f)
        {
            if (shader == null)
                return;

            volume *= shader.GetVolume();
            pitch *= shader.GetPitch();

            var sound = shader.RandomSound;
            Raylib.SetSoundVolume(sound, volume);
            Raylib.SetSoundPitch(sound, pitch);
            Raylib.PlaySound(sound);
        }
    }
}
