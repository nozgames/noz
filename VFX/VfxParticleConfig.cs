/*

    Copyright (c) 2024 NoZ Games, LLC. All rights reserved.

*/

using NoZ.Graphics;

namespace NoZ.VFX
{
    internal struct VfxRandomRange
    {
        public float Min;
        public float Max;
        
        public float GetRandomValue() => Randomizer.Range(Min, Max);
    }

    internal struct VfxRandomRangeInt
    {
        public int Min;
        public int Max;
        
        public int GetRandomValue() => Randomizer.Range(Min, Max + 1);
    }

    internal struct VfxRandomColor
    {
        public Color Min;
        public Color Max;
        
        public Color GetRandomValue() => MathEx.Lerp(Min, Max, Randomizer.Next());
    }
    
    internal struct VfxParticleConfig
    {
        public VfxRandomRange Duration;
        public VfxRandomRange StartSize;
        public VfxRandomRange EndSize;
        public VfxRandomRange StartSpeed;
        public VfxRandomRange EndSpeed;
        public VfxRandomRange StartRotation;
        public VfxRandomRange Rotation;
        public VfxRandomColor StartColor;
        public VfxRandomColor EndColor;
        public Sprite Sprite;
        public Animation Animation;
        public bool AnimationLoop;
        public VfxRandomRange AnimationSpeed;
        public VfxRandomRange AnimationStartTime;
        public int SortOrder;

        // TODO: Remove Raylib_cs dependencies and implement SDL3 VFX logic as needed.
    }
}
