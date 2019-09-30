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

namespace NoZ
{

    public delegate Color GetPixelDelegate(byte[] raw, int x, int y, int width);

    public delegate void SetPixelDelegate(byte[] raw, int x, int y, int width, Color color);

    public class PixelData
    {
        private GetPixelDelegate _getPixel;
        private SetPixelDelegate _setPixel;

        public int Width { get; private set; }
        public int Height { get; private set; }
        public int BytesPerPixel { get; private set; }

        public byte[] Raw { get; private set; }

        public void SetPixel(int x, int y, Color color) => _setPixel(Raw, x, y, Width, color);

        public Color GetPixel(int x, int y) => _getPixel(Raw, x, y, Width);

        public PixelData(int width, int height, int bytesPerPixel, byte[] raw, GetPixelDelegate getPixel, SetPixelDelegate setPixel)
        {
            Width = width;
            Height = height;
            BytesPerPixel = bytesPerPixel;
            Raw = raw;
            _setPixel = setPixel;
            _getPixel = getPixel;
        }

        public void Clear(Color color)
        {
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    _setPixel(Raw, x, y, Width, color);
        }
    }
}
