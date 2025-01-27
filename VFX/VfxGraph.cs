/*

    Copyright (c) 2024 NoZ Games, LLC. All rights reserved.

*/

using NoZ.Graphics;

namespace NoZ.VFX
{
    public class VfxGraph : Resource<VfxGraph>
    {
        internal VfxEmitterConfig[]? _emitters;
        internal SpriteAtlas? _atlas;

        protected internal override void Unload()
        {            
        }
    }
}
