/*
  NozEngine Library

  Copyright(c) 2015 NoZ Games, LLC

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

using System.Collections.Generic;
using System.IO;

namespace NoZ
{
    public class ImageAnimation : Resource
    {
        public struct Frame
        {
            public float Start;
            public float Duration;
            public Image Image;
        }

        public Frame[] Frames { get; private set; }

        /// <summary>
        /// Total duration of the image animation
        /// </summary>
        public float Duration { get; private set; }

        public ImageAnimation(string name) : base(name)
        {
        }

        public static ImageAnimation Create(string name, BinaryReader reader)
        {
            var anim = new ImageAnimation(name);
            var frameCount = reader.ReadInt32();
            anim.Frames = new Frame[frameCount];
            var duration = 0.0f;
            for (int i = 0; i < frameCount; i++)
            {
                var frame = new Frame();
                frame.Image = ResourceDatabase.Load<Image>(reader.ReadString());
                frame.Duration = reader.ReadSingle();
                frame.Start = duration;
                anim.Frames[i] = frame;
                duration += frame.Duration;
            }

            anim.Duration = duration;

            return anim;
        }

        /// <summary>
        /// Get the frame index for the given frame time
        /// </summary>
        /// <param name="time">Frame time</param>
        /// <param name="hint">Hint frame</param>
        /// <returns>Frame index</returns>
        public int GetFrameIndex (float time, int hint=-1)
        {
            // Past end?
            if (time >= Duration)
                return Frames.Length - 1;

            hint = hint >= 0 ? hint : 0;

            // Hint?
            while (hint < Frames.Length - 1 && time >= Frames[hint+1].Start) hint++;
            while (hint > 0 && time < Frames[hint].Start) hint--;
            return hint;
        }
    }
}
