
namespace NoZ {
    public interface IAudioDriver {
        AudioClip CreateClip();

        AudioClip CreateClip(int samples, AudioChannelFormat channelFormat, int frequency);

        Voice Play(AudioClip clip);

        bool IsPlaying(Voice voice);

        void DoFrame();

        // Dispose of the audio driver
        void Dispose();
    }
}
