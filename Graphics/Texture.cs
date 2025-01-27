/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

namespace NoZ.Graphics
{
    public class Texture : Resource<Texture>
    {
        internal Raylib_cs.Texture2D _texture;

        public int Width => _texture.Width;
        public int Height => _texture.Height;

        public Texture(int width, int height, Raylib_cs.Color color)
        {
            var image = Raylib_cs.Raylib.GenImageColor(width, height, Raylib_cs.Color.White);
            _texture = Raylib_cs.Raylib.LoadTextureFromImage(image);
            Raylib_cs.Raylib.UnloadImage(image);
        }

        internal Texture(Raylib_cs.Texture2D texture)
        {
            _texture = texture;
        }

        protected internal override void Unload()
        {
            Raylib_cs.Raylib.UnloadTexture(_texture);
        }
    }
}
