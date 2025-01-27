/*

    Copyright (c) 2024 NoZ Games, LLC. All rights reserved.

*/

#if false
using NoZ.ECS;
using System.Numerics;

namespace NoZ.Tweening
{
    public static class TweenSystem
    {
        private static EntityCollection<Tween>? _tweens;

        public static void Initialize()
        {
            _tweens = new EntityCollection<Tween>(1024);
        }

        public static void Shutdown()
        {
        }

        public static void Update()
        {
            var tweens = _tweens!.Entities;
            var deltaTime = Time.DeltaTime;
            foreach(ref var tween in tweens)
                UpdateTween(ref tween, tween.Elapsed + deltaTime);

            _tweens.Collect();
        }

        private static float Ease(EaseType ease, float time, in Vector4 easeParams) => ease switch
        {
            EaseType.Cubic => Easing.Cubic(time),
            EaseType.Back => Easing.Back(time, easeParams.X),
            EaseType.Quadratic => Easing.Quadratic(time),
            _ => time
        };

        internal static void PlayTween(ref Tween tween)
        {
            if (tween.IsPlaying)
                return;

            // Set the state based on the update mode
            var oldState = tween.State;
            tween.State = TweenState.Playing;

            // Handle the first time play is called
            if (oldState == TweenState.Created)
                tween.Elapsed = 0.0f;

            // Force OnPlay to be called again by clearing the evaluating flag
            tween.ClearFlags(TweenFlags.Evaluating);

            UpdateTween(ref tween, tween.Elapsed);
        }

        private static void UpdateTween(ref Tween tween, float time)
        {
            if (tween.State == TweenState.Free)
            {
                _tweens!.Destroy(tween.GetId());
                return;
            }

            var clampedTime = Math.Clamp(time, 0, tween.Delay + tween.Duration);
            tween.Elapsed = clampedTime;

            var normalizedTime = Math.Clamp((tween.Elapsed - tween.Delay) / tween.Duration, 0, 1);

            // Ping pong
            if (tween.IsPingPong)
            {
                if (normalizedTime >= 0.5f)
                    normalizedTime = 1.0f - ((normalizedTime - 0.5f) / 0.5f);
                else
                    normalizedTime = normalizedTime / 0.5f;
            }

            // Ease In / Out
            if (tween.EaseIn != EaseType.None && tween.EaseOut != EaseType.None)
            {
                if (normalizedTime <= 0.5f)
                    normalizedTime = Ease(tween.EaseIn, normalizedTime * 2f, tween.EaseInParams) * 0.5f;
                else if (normalizedTime > 0.5f)
                    normalizedTime = (1f - Ease(tween.EaseOut, (1f - normalizedTime) * 2f, tween.EaseOutParams)) * 0.5f + 0.5f;
            }
            // Ease In
            else if (tween.EaseIn != EaseType.None)
                normalizedTime = Ease(tween.EaseIn, normalizedTime, tween.EaseInParams);
            // Ease Out
            else if (tween.EaseOut != EaseType.None)
                normalizedTime = 1f - Ease(tween.EaseOut, 1f - normalizedTime, tween.EaseOutParams);

            tween.Value = Vector4.Lerp(tween.From, tween.To, normalizedTime);

            // Not done?
            if (time < tween.Duration + tween.Delay)
                return;

            // Loop?
            if (tween.IsLooping)
            {
                // Clear the delay after looping
                tween.Delay = 0f;

                // Loop count
                if (tween.LoopCount > 0)
                {
                    tween.LoopCount--;
                    if (tween.LoopCount == 0)
                        tween.IsLooping = false;
                }

                // Restart time at zero
                RestartTween(ref tween);

                // Evaluate using any extra time we had
                UpdateTween(ref tween, time - clampedTime);
                return;
            }

            // When a non-element is done and has auto stop enabled free it
            if (tween.IsAutoStop)
                FreeTween(ref tween);
        }

        private static void RestartTween(ref Tween tween)
        {           
            tween.ClearFlags(TweenFlags.Evaluating);
            tween.Elapsed = 0.0f;
        }

        private static void FreeTween(ref Tween tween)
        {
            if (tween.State == TweenState.Free)
                return;

            tween.State = TweenState.Free;
        }

        private static EntityRef<Tween> CreateDefault()
        {
            var tweenRef = _tweens!.Create();
            ref var tween = ref tweenRef.Value;
            tween.From = Vector4.Zero;
            tween.To = Vector4.Zero;
            tween.Duration = 1;
            tween.Delay = 0;
            tween.Elapsed = 0;
            tween.Flags = TweenFlags.AutoStop;
            tween.State = TweenState.Created;
            return tweenRef;
        }

        public static EntityRef<Tween> Create() => Create(0, 1);

        public static EntityRef<Tween> Create(float from, float to)
        {
            var tweenRef = CreateDefault();
            ref var tween = ref tweenRef.Value;
            tween.From = new Vector4(from, 0, 0, 0);
            tween.To = new Vector4(to, 0, 0, 0);
            return tweenRef;
        }

        public static EntityRef<Tween> Create(in Vector4 from, in Vector4 to)
        {
            var tweenRef = CreateDefault();
            ref var tween = ref tweenRef.Value;
            tween.From = from;
            tween.To = to;
            return tweenRef;
        }

        public static EntityRef<Tween> Create(in Vector3 from, in Vector3 to)
        {
            var tweenRef = CreateDefault();
            ref var tween = ref tweenRef.Value;
            tween.From = new Vector4(from.X, from.Y, from.Z, 0);
            tween.To = new Vector4(to.X, to.Y, to.Z, 0);
            return tweenRef;
        }

        public static EntityRef<Tween> Create(Vector2 from, Vector2 to)
        {
            var tweenRef = CreateDefault();
            ref var tween = ref tweenRef.Value;
            tween.From = new Vector4(from.X, from.Y, 0, 0);
            tween.To = new Vector4(to.X, to.Y, 0, 0);
            return tweenRef;
        }

        public static bool TryGetTween(TweenId id, out EntityRef<Tween> tweenRef) =>
            _tweens!.TryGet(id.Id, out tweenRef);
    }
}

#endif
