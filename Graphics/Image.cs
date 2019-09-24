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
using System.IO;

namespace NoZ.Graphics
{
    public abstract class Image : Resource
    {
        /// <summary>
        /// Create a blank image 
        /// </summary>
        /// <param name="name">Optional name of image</param>
        /// <param name="width">Width of image</param>
        /// <param name="height">Height of image</param>
        /// <param name="format">Format of image</param>
        /// <returns>Created image</returns>
        public static Image Create (string name, int width, int height, PixelFormat format) {
            return Window.Graphics.CreateImage(name, width, height, format);
        }

        /// <summary>
        /// Create a named image from a stream
        /// </summary>
        /// <param name="name">Name of image</param>
        /// <param name="reader">Stream to read from</param>
        /// <returns>Created image</returns>
        public static Image Create (string name, BinaryReader reader)
        {
            // Create the image
            var image = Create(name, reader.ReadInt16(), reader.ReadInt16(), (PixelFormat)reader.ReadByte());

            image.Border = reader.ReadThickness();

            byte[] dst = image.LockBytes();
            reader.Read(dst, 0, image.Stride * image.Height);
            image.UnlockBytes();

            return image;
        }

        protected Image(string name) : base(name) {
        }

        protected Image(string name, int width, int height, PixelFormat format) : base (name) {
            Width = width;
            Height = height;
            PixelFormat = format;
            Stride = BytesPerPixel * Width;
        }

        /// <summary>
        /// Image border used by nodes that render the image in pieces in 
        /// order to preserve the images edge integrity when resized.
        /// </summary>
        public Thickness Border { get; set; }

        /// <summary>
        /// Width of the image in pixels
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Height of the image in pixels
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Number of bytes in a single row of pixels
        /// </summary>
        public int Stride { get; private set; }

        /// <summary>
        /// Format of each pixel
        /// </summary>
        public PixelFormat PixelFormat { get; private set; }

        /// <summary>
        /// Width and height in vector form
        /// </summary>
        public Vector2Int Size => new Vector2Int(Width, Height);

        public int BytesPerPixel {
            get {
                switch (PixelFormat) {
                    case PixelFormat.A8:
                        return 1;
                    case PixelFormat.R8G8B8:
                        return 3;
                    case PixelFormat.R8G8B8A8:
                        return 4;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        private static Color GetPixelR8G8B8A8 (byte[] raw, int x, int y, int width) {
            int offset = x * 4 + y * width * 4;
            return Color.FromRgba(raw[offset], raw[offset+1], raw[offset+2], raw[offset+3]);
        }

        private static Color GetPixelR8G8B8 (byte[] raw, int x, int y, int width) {
            int offset = x * 3 + y * width * 3;
            return Color.FromRgba(raw[offset], raw[offset + 1], raw[offset + 2], 255);       
        }

        private static Color GetPixelA8(byte[] raw, int x, int y, int width) {
            int offset = x + y * width;
            return Color.FromRgba(0,0,0, raw[offset]);
        }

        private static void SetPixelR8G8B8A8(byte[] raw, int x, int y, int width, Color color) {
            int offset = x * 4 + y * width * 4;
            raw[offset] = color.R;
            raw[offset + 1] = color.G;
            raw[offset + 2] = color.B;
            raw[offset + 3] = color.A;
        }

        private static void SetPixelR8G8B8(byte[] raw, int x, int y, int width, Color color) {
            int offset = x * 3 + y * width * 3;
            raw[offset] = color.R;
            raw[offset + 1] = color.G;
            raw[offset + 2] = color.A;
        }

        private static void SetPixelA8(byte[] raw, int x, int y, int widtb, Color color) {
            int offset = x + y * widtb;
            raw[offset] = color.A;
        }

        public PixelData Lock() {
            switch(PixelFormat) {
                case PixelFormat.A8: return new PixelData(Width, Height, BytesPerPixel, LockBytes(), GetPixelA8, SetPixelA8); 
                case PixelFormat.R8G8B8: return new PixelData(Width, Height, BytesPerPixel, LockBytes(), GetPixelR8G8B8, SetPixelR8G8B8); 
                case PixelFormat.R8G8B8A8: return new PixelData(Width, Height, BytesPerPixel, LockBytes(), GetPixelR8G8B8A8, SetPixelR8G8B8A8); 
                default:
                    throw new NotImplementedException();
            }
        }

        public void Unlock() {
            UnlockBytes();
        }
        
        /// <summary>
        /// Save the image to the given stream
        /// </summary>
        /// <param name="writer">Stream to save image to</param>
        public void Save (BinaryWriter writer)
        {
            writer.Write((short)Width);
            writer.Write((short)Height);
            writer.Write((byte)PixelFormat);
            writer.Write(Border);

            var bytes = LockBytes();
            writer.Write(bytes, 0, bytes.Length);
            UnlockBytes();
        }

        protected abstract byte[] LockBytes();

        protected abstract void UnlockBytes();
    }
}
