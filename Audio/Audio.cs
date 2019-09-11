using System;

namespace NoZ {

    public static class Audio {
        public static Voice Play(AudioClip clip) {
            return Game.AudioDriver?.Play(clip) ?? Voice.Error;
        }

        public static bool IsPlaying(Voice voice) {
            return Game.AudioDriver.IsPlaying(voice);
        }
    }
}
