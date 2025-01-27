/*

    Copyright (c) 2024 NoZ Games, LLC. All rights reserved.

*/

#if false
using NoZ.ECS;
using System.Numerics;

namespace NoZ.Tweening
{
    public struct Tween : IEntity<Tween>
    {
        internal Entity Entity;
        internal TweenFlags Flags;
        internal TweenState State;
        internal float Delay;
        internal float Duration;
        internal float Elapsed;
        internal Vector4 Value;
        internal Vector4 From;
        internal Vector4 To;
        internal EaseType EaseIn;
        internal EaseType EaseOut;
        internal Vector4 EaseInParams;
        internal Vector4 EaseOutParams;
        internal int LoopCount;

        public bool IsPlaying => State == TweenState.Playing || State == TweenState.Free;
        public bool IsPingPong
        {
            get => HasFlags(TweenFlags.PingPong);
            internal set => SetFlags(TweenFlags.PingPong, value);
        }
        public bool IsLooping
        {
            get => HasFlags(TweenFlags.Looping);
            internal set => SetFlags(TweenFlags.Looping, value);
        }

        public bool IsAutoStop => HasFlags(TweenFlags.AutoStop);
        public bool IsCreated => State == TweenState.Created;

        internal bool HasFlags(TweenFlags flags) => (Flags & flags) == flags;

        internal void SetFlags(TweenFlags flags, bool value = true)
        {
            if (value)
                Flags |= flags;
            else
                ClearFlags(flags);
        }

        internal void ClearFlags(TweenFlags flags) => Flags &= ~(flags);

        internal Vector2 AsVector2() => new Vector2(Value.X, Value.Y);

        internal Vector3 AsVector3() => new Vector3(Value.X, Value.Y, Value.Z);

        internal float AsFloat() => Value.X;
    }

    public static class TweenExtensions
    {
        public static EntityRef<Tween> Duration(this EntityRef<Tween> tween, float duration)
        {
            ref var tweenValue = ref tween.Value;
            tweenValue.Duration = duration;
            return tween;
        }

        public static EntityRef<Tween> Delay(this EntityRef<Tween> tween, float delay)
        {
            ref var tweenValue = ref tween.Value;
            tweenValue.Delay = delay;
            return tween;
        }

        public static EntityRef<Tween> PingPong(this EntityRef<Tween> tween)
        {
            ref var tweenValue = ref tween.Value;
            tweenValue.IsPingPong = true;
            return tween;
        }

        public static EntityRef<Tween> EaseOutCubic(this EntityRef<Tween> tween)
        {
            ref var tweenValue = ref tween.Value;
            tweenValue.EaseOut = EaseType.Cubic;
            return tween;
        }

        public static EntityRef<Tween> EaseInCubic(this EntityRef<Tween> tween)
        {
            ref var tweenValue = ref tween.Value;
            tweenValue.EaseOut = EaseType.Cubic;
            return tween;
        }

        public static EntityRef<Tween> EaseInOutCubic(this EntityRef<Tween> tween)
        {
            ref var tweenValue = ref tween.Value;
            tweenValue.EaseIn = EaseType.Cubic;
            tweenValue.EaseOut = EaseType.Cubic;
            return tween;
        }

        public static EntityRef<Tween> EaseInOutQuadratic(this EntityRef<Tween> tween)
        {
            ref var tweenValue = ref tween.Value;
            tweenValue.EaseIn = EaseType.Quadratic;
            tweenValue.EaseOut = EaseType.Quadratic;
            return tween;
        }

        public static EntityRef<Tween> EaseOutBack(this EntityRef<Tween> tween, float size)
        {
            ref var tweenValue = ref tween.Value;
            tweenValue.EaseOutParams = new Vector4(size, 0, 0, 0);
            tweenValue.EaseOut = EaseType.Back;
            return tween;
        }

        public static TweenId Play(this EntityRef<Tween> tween)
        {
            ref var tweenValue = ref tween.Value;            
            TweenSystem.PlayTween(ref tweenValue);
            return new TweenId { Id = tweenValue.GetId() };
        }

        public static bool IsPlaying(this TweenId tweenId)
        {
            return TweenSystem.TryGetTween(tweenId, out var tweenRef) && tweenRef.Value.IsPlaying;
        }

        public static Vector2 AsVector2(this TweenId tweenId) =>
            TweenSystem.TryGetTween(tweenId, out var tweenRef)
                ? tweenRef.Value.AsVector2() : Vector2.Zero;

        public static Vector3 AsVector3(this TweenId tweenId) =>
            TweenSystem.TryGetTween(tweenId, out var tweenRef)
                ? tweenRef.Value.AsVector3() : Vector3.Zero;

        public static float AsFloat(this TweenId tweenId, float defaultValue=0.0f) =>
            TweenSystem.TryGetTween(tweenId, out var tweenRef)
                ? tweenRef.Value.AsFloat()
                : defaultValue;

    }
}

#endif
