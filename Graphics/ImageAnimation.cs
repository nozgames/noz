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
        public List<Image> Frames { get; private set; } = new List<Image>();
        public int FramesPerSecond = 30;
        public bool Looping { get; set; } = true;

        public ImageAnimation(string name) : base(name)
        {
        }

        public static ImageAnimation Create(string name, BinaryReader reader)
        {
            var anim = new ImageAnimation(name);
            anim.FramesPerSecond = reader.ReadInt32();
            anim.Looping = reader.ReadBoolean();
            var frameCount = reader.ReadInt32();
            anim.Frames.Capacity = frameCount;
            for (int i = 0; i < frameCount; i++)
            {
                anim.Frames.Add(ResourceDatabase.Load<Image>(reader.ReadString()));
                var duration = reader.ReadSingle();
            }

            // TODO: remove
            if (anim.FramesPerSecond == 0)
                anim.FramesPerSecond = 10;
            anim.Looping = true;

            return anim;
        }
    }
}
