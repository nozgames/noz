using System;

namespace NoZ {

    public delegate Color GetPixelDelegate(byte[] raw, int x, int y, int width);

    public delegate void SetPixelDelegate(byte[] raw, int x, int y, int width, Color color);

    public class PixelData {
        private GetPixelDelegate _getPixel;
        private SetPixelDelegate _setPixel;

        public int Width { get; private set; }
        public int Height { get; private set; }
        public int BytesPerPixel { get; private set; }

        public byte[] Raw { get; private set; }

        public void SetPixel(int x, int y, Color color) => _setPixel(Raw, x, y, Width, color);

        public Color GetPixel(int x, int y) => _getPixel(Raw, x, y, Width);

        public PixelData (int width, int height, int bytesPerPixel, byte[] raw, GetPixelDelegate getPixel, SetPixelDelegate setPixel) {
            Width = width;
            Height = height;
            BytesPerPixel = bytesPerPixel;
            Raw = raw;
            _setPixel = setPixel;
            _getPixel = getPixel;
        }

        public void Clear (Color color) {
            for (int y=0; y<Height; y++)
                for(int x =0; x<Width ; x++)
                    _setPixel(Raw, x, y, Width, color);
        }
    }
}
