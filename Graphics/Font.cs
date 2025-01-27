/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

namespace NoZ.Graphics
{
    public class Font : Resource<Font>
    {
        internal Raylib_cs.Font _font;

        internal Font(Raylib_cs.Font font)
        {
            _font = font;
        }

        protected internal override void Unload()
        {
            Raylib_cs.Raylib.UnloadFont(_font);
        }
    }
}
