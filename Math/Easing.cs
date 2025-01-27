/*

    Copyright (c) 2024 NoZ Games, LLC. All rights reserved.

*/

using System.Numerics;
using System.Runtime.CompilerServices;

namespace NoZ
{
    public static class Easing
    {
        public static float Quadratic(float t) => t * t;

        public static float Cubic(float t) => t * t * t;

        public static float Back(float t, float p) => MathF.Pow(t, 3f) - t * MathF.Max(0f, p) * MathF.Sin(MathF.PI * t);

        public static float Circle(float t) => 1.0f - MathF.Sqrt(1.0f - t * t);

        public static float Exponential(float t, float p) =>
            p == 0.0f ? t : ((MathF.Exp(p * t) - 1.0f) / (MathF.Exp(p) - 1.0f));

        public static float Sine(float t) => 1.0f - MathF.Sin(MathF.PI * 0.5f * (1f - t));

        public static float CubicBezier(float t, float x1, float y1, float x2, float y2)
        {
            var oneMinusT = 1.0f - t;
            var oneMinusT_2 = oneMinusT * oneMinusT;
            var oneMinusT_3 = oneMinusT_2 * oneMinusT;
            var t_2 = t * t;
            var t_3 = t_2 * t;

            var p0 = Vector2.Zero;
            var p1 = new Vector2(x1, y1);
            var p2 = new Vector2(x2, y2);
            var p3 = Vector2.One;

            var r =
                p0 * oneMinusT_3 +
                3.0f * p1 * t * oneMinusT_2 +
                3.0f * p2 * t_2 * oneMinusT +
                p3 * t_3;

            return r.Y;
        }

        public static float Bounce(float t, float bounces, float bounciness)
        {
            var pow = MathF.Pow(bounciness, bounces);
            var invBounciness = 1f - bounciness;

            var sum_units = (1f - pow) / invBounciness + pow * 0.5f;
            var unit_at_t = t * sum_units;

            var bounce_at_t = MathF.Log(-unit_at_t * invBounciness + 1f, bounciness);
            var start = MathF.Floor(bounce_at_t);
            var end = start + 1f;

            var div = 1f / (invBounciness * sum_units);
            var start_time = (1f - MathF.Pow(bounciness, start)) * div;
            var end_time = (1f - MathF.Pow(bounciness, end)) * div;

            var mid_time = (start_time + end_time) * 0.5f;
            var peak_time = t - mid_time;
            var radius = mid_time - start_time;
            var amplitude = MathF.Pow(1f / bounciness, bounces - start);

            return (-amplitude / (radius * radius)) * (peak_time - radius) * (peak_time + radius);
        }

        public static float Elastic(float t, float oscillations, float springiness)
        {
            oscillations = MathF.Max(0, (int)oscillations);
            springiness = MathF.Max(0f, springiness);

            float expo;
            if (springiness == 0f)
                expo = t;
            else
                expo = (MathF.Exp(springiness * t) - 1f) / (MathF.Exp(springiness) - 1f);

            return expo * (MathF.Sin((MathF.PI * 2f * oscillations + MathF.PI * 0.5f) * t));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float BackInOut(float t, float amountIn, float amountOut) =>
            t <= 0.5f 
                ? Back(t * 2f, amountIn) * 0.5f
                : (1f - Back((1f - t) * 2f, amountOut)) * 0.5f + 0.5f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float BackOut(float t, float amount) =>
            1f - Back(1f - t, amount);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float QuadraticInOut(float t) =>
            t <= 0.5f
                ? Quadratic(t * 2f) * 0.5f
                : (1f - Quadratic((1f - t) * 2f)) * 0.5f + 0.5f;
    }
}
