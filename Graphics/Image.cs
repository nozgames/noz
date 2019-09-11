/*
  NoZ Game Engine

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

using System;
using System.IO;

using NoZ.Serialization;

namespace NoZ {

    [SharedResource]
    [SerializedType(Allocator = typeof(ImageAllocator))]
    [Version(1)]
    public abstract class Image : ISerializedType, IResource {
        public static Image Load (string filename) {
            using (var stream = File.OpenRead(filename)) {
                return Game.GraphicsDriver.LoadImage (stream);
            }
        }

        public static Image Load (Stream stream) {
            return Game.GraphicsDriver.LoadImage(stream);
        }

        public static Image Create (int width, int height, PixelFormat format) {
            return Game.GraphicsDriver.CreateImage(width, height, format);
        }

        protected Image() {
        }

        protected Image(int width, int height, PixelFormat format) {
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

        public int Width { get; private set; }

        public int Height { get; private set; }

        public int Stride { get; private set; }

        public PixelFormat PixelFormat { get; private set; }

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

        Resource IResource.Resource { get; set; }

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


        void ISerializedType.Deserialize(BinaryDeserializer reader) {
            Border = reader.ReadThickness();
            Width = reader.ReadInt16();
            Height = reader.ReadInt16();
            PixelFormat = (PixelFormat)reader.ReadByte();
            Stride = Width * BytesPerPixel;

            byte[] dst = LockBytes();
            reader.ReadBytes(dst, 0);
            UnlockBytes();
        }

        void ISerializedType.Serialize(BinarySerializer writer) {
            writer.WriteThickness(Border);
            writer.WriteInt16((short)Width);
            writer.WriteInt16((short)Height);
            writer.WriteByte((byte)PixelFormat);

            writer.WriteBytes(LockBytes());
            UnlockBytes();
        }

        private static class ImageAllocator {
            public static object CreateInstance() => Game.GraphicsDriver.CreateImage();
        }

        protected abstract byte[] LockBytes();

        protected abstract void UnlockBytes();
    }
}
