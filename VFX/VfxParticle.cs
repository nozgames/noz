/*

    Copyright (c) 2024 NoZ Games, LLC. All rights reserved.

*/

using System.Numerics;

namespace NoZ.VFX
{
#if false
    internal struct VfxParticle : IEntity<VfxParticle>
    {
        public Entity Entity;
        public Transform Transform;
        public SpriteRendererOld Sprite;
        public Animator Animator;
        public Vector2 Speed;
        public Vector2 Size;
        public Vector2 Rotation;
        public float Elapsed;
        public float Duration;
        public float DurationInv;
        public Vector3 Direction;
        public Color StartColor;
        public Color EndColor;
        public int BaseSortOrder;

        // TODO: Remove Raylib_cs dependencies and implement SDL3 VFX logic as needed.
    }
#endif
}
