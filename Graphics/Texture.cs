/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

using SDL3;

namespace NoZ.Graphics
{
    // SDL3 implementation for Texture
    public class Texture : Resource<Texture>
    {
        public int Width { get; }
        public int Height { get; }
        
        internal nint Handle { get; private set; }

        public Texture(int width, int height) : this(width, height, new Color(255,255,255,255)) { }
        
        public Texture(int width, int height, Color color)
        {
            Width = width;
            Height = height;
            Handle = 0;
        }

        public Texture(nint handle, int width, int height)
        {
            Handle = handle;
            Width = width;
            Height = height;
        }

        protected internal override void Unload()
        {
            if (Handle == nint.Zero)
                return;

            SDL.DestroyTexture(Handle);
            Handle = nint.Zero;
        }
    }
}
