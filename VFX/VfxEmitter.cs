/*

    Copyright (c) 2024 NoZ Games, LLC. All rights reserved.

*/

#if false
using NoZ.ECS;

using Transform = NoZ.ECS.Transform;

namespace NoZ.VFX
{
    internal struct VfxEmitter : IEntity<VfxEmitter>
    {
        public Entity Entity;
        public Transform Transform;
        public float Elapsed;
        public float Duration;
        public float EmitRate;
        public VfxEmitterConfig Config;
        public VfxParticleConfig Particle;
    }
}

#endif
