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

namespace NoZ
{
    public static class Audio
    {
        public static IAudioDriver Driver { get; set; }

        /// <summary>
        /// Get/Set the main volume
        /// </summary>
        public static float Volume {
            get => Driver.Volume;
            set => Driver.Volume = value;
        }

        /// <summary>
        /// Play the given audio clip
        /// </summary>
        /// <param name="clip">Clip to play</param>
        /// <returns>Unique voice that represents the playing clip</returns>
        public static Voice Play(AudioClip clip) => Driver.Play(clip);

        /// <summary>
        /// Returns true if the give voice is playing
        /// </summary>
        /// <param name="voice">Voice</param>
        /// <returns>True if playing</returns>
        public static bool IsPlaying(Voice voice) => Driver.IsPlaying(voice);
    }
}
