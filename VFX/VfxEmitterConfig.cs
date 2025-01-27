/*

    Copyright (c) 2024 NoZ Games, LLC. All rights reserved.

*/

using System.Numerics;

namespace NoZ.VFX
{
    internal struct VfxEmitterConfig
    {
        public int Rate;
        public VfxRandomRangeInt Burst;
        public VfxRandomRange Duration;
        public VfxRandomRange Angle; 
        public VfxRandomRange Radius;
        public Vector3 Min;
        public Vector3 Max;
        public VfxRandomRange X;
        public VfxRandomRange Y;
        public VfxParticleConfig Particle;
    }
}