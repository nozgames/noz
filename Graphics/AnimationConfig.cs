/*

    Copyright (c) 2024 NoZ Games, LLC. All rights reserved.

*/

using NoZ.Helpers;

namespace NoZ.Graphics
{
    public struct AnimationFrameConfig
    {
        public required string Sprite;
        public int Hold;
        public string Event;
    }
    
    public struct AnimationConfig
    {
        public float? Speed;
        public AnimationFrameConfig[] Frames;
        
        public unsafe Animation? LoadAnimation(SpriteAtlas atlas, Type? eventEnumType)
        {
            var animation = new Animation { FrameCount = Frames.Length, Speed = Speed ?? 1.0f };
            var framePtr = &animation.Frame1;
            var frameCount = 0;
            for (var i = 0; i < Frames.Length; i++)
            {
                ref var frameConfig = ref Frames[i];
                framePtr->Sprite = atlas.LoadSprite(frameConfig.Sprite);
                framePtr->Hold = Math.Max(1, frameConfig.Hold);
                framePtr->Event = eventEnumType != null ? Enum.TryParse(eventEnumType, frameConfig.Event, out var result) ? (int)result : 0 : 0;
                frameCount += framePtr->Hold;
                framePtr++;
            }

            //animation.Length = AnimationHelper.GetAnimationLength(frameCount);
            
            return animation;
        }
    }
}
