/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

namespace NoZ.Graphics
{
    // SDL3 stub for Font
    public class Font : Resource<Font>
    {
        public int BaseSize { get; set; } = 16;
        // TODO: Add SDL3 font fields as needed

        public Font(int baseSize = 16)
        {
            BaseSize = baseSize;
        }

        protected internal override void Unload()
        {
            // TODO: Unload SDL3 font here
        }
    }
}
