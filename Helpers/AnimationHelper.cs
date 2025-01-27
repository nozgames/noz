/*

    Copyright (c) 2024 NoZ Games, LLC. All rights reserved.

*/

#if false

using System.Runtime.CompilerServices;
using NoZ;
using NoZ.Renderer;

namespace NoZ.Helpers
{
    public static unsafe class AnimationHelper
    {
        private const int FramesPerSecond = 12;
        private const float FrameTime = 1.0f / FramesPerSecond;

        private static KeyFrame* GetFrame(ref Animation animation, int frame) =>
            ((KeyFrame*)((byte*)Unsafe.AsPointer(ref animation) + Animation.FrameOffset)) + frame;
        
        public static void Update(ref Animator animator, ref SpriteRendererOld sprite)
        {
            if (animator.Animation.FrameCount == 0)
                return;

            var frame = GetFrame(ref animator.Animation, animator.Frame);
            
            animator.Elapsed += Time.DeltaTime * animator.Speed * animator.Animation.Speed;
            animator.Event = 0;
            if (animator.Elapsed < frame->Hold * FrameTime)
                return;

            var newFrame = animator.Frame + 1;
            if (animator.Loop)
                newFrame %= animator.Animation.FrameCount;
            else
            {
                animator.Playing = newFrame < animator.Animation.FrameCount;
                newFrame = Math.Min(newFrame, animator.Animation.FrameCount - 1);
            }

            animator.Elapsed = 0.0f;

            if (newFrame == animator.Frame)
                return;

            animator.Frame = newFrame;

            frame = GetFrame(ref animator.Animation, animator.Frame);
            animator.Event = frame->Event;
            sprite.Sprite = frame->Sprite;
        }

        public static void Play(ref Animator animator, ref SpriteRendererOld sprite, in Animation animation, bool loop = false, float speed = 1.0f, float normalizedTime = 0.0f)
        {
            var frame = GetFrame(ref animator.Animation, 0);
            
            animator.Animation = animation;
            animator.Frame = 0;
            animator.Elapsed = 0.0f;
            animator.Loop = loop;
            animator.Playing = true;
            animator.Speed = speed;

            if (normalizedTime <= 0.0f)
            {
                sprite.Sprite = frame->Sprite;
                animator.Event = frame->Event;
                return;
            }
            
            var time = normalizedTime * animator.Animation.Length;
            for (int i=0, c=animator.Animation.FrameCount; i<c; i++)
            {
                frame = GetFrame(ref animator.Animation, i);
                if (time < frame->Hold * FrameTime)
                {
                    animator.Frame = i;
                    animator.Elapsed = time;
                    animator.Event = frame->Event;
                    sprite.Sprite = frame->Sprite;
                    break;
                }

                time -= frame->Hold * FrameTime;
            }
        }

        public static float GetAnimationLength(int frameCount) => frameCount * FrameTime;
    }
}

#endif
