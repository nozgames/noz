/*

    Copyright (c) 2024 NoZ Games, LLC. All rights reserved.

*/

using System.Numerics;
using NoZ.Graphics;

namespace NoZ
{
    public static class MathEx
    {
        public static float RAD2DEG = 180.0f / MathF.PI;
        public static float DEG2RAD = MathF.PI / 180.0f;

        public static int NextPowerOfTwo(int value)
        {
            value -= 1;
            value |= value >> 16;
            value |= value >> 8;
            value |= value >> 4;
            value |= value >> 2;
            value |= value >> 1;
            return value + 1;
        }

        /// <summary>
        /// Smoothly damps the camera position towards the target position.
        /// </summary>
        public static Vector2 SmoothDamp(in Vector2 current, Vector2 target, ref Vector2 currentVelocity, float smoothTime, float deltaTime)
            => SmoothDamp(current, target, ref currentVelocity, smoothTime, float.MaxValue, deltaTime);
        
        /// <summary>
        /// Smoothly damps the camera position towards the target position.
        /// </summary>
        public static Vector2 SmoothDamp(in Vector2 current, Vector2 target, ref Vector2 currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
        {
            var outputX = 0f;
            var outputY = 0f;

            // Based on Game Programming Gems 4 Chapter 1.10
            smoothTime = Math.Max(0.0001F, smoothTime);
            var omega = 2F / smoothTime;
            var x = omega * deltaTime;
            var exp = 1F / (1F + x + 0.48F * x * x + 0.235F * x * x * x);
            var changeX = current.X - target.X;
            var changeY = current.Y - target.Y;
            var originalTo = target;

            // Clamp maximum speed
            var maxChange = maxSpeed * smoothTime;

            var maxChangeSq = maxChange * maxChange;
            var sqrmag = changeX * changeX + changeY * changeY;
            if (sqrmag > maxChangeSq)
            {
                var mag = (float)Math.Sqrt(sqrmag);
                changeX = changeX / mag * maxChange;
                changeY = changeY / mag * maxChange;
            }

            target.X = current.X - changeX;
            target.Y = current.Y - changeY;

            var tempX = (currentVelocity.X + omega * changeX) * deltaTime;
            var tempY = (currentVelocity.Y + omega * changeY) * deltaTime;

            currentVelocity.X = (currentVelocity.X - omega * tempX) * exp;
            currentVelocity.Y = (currentVelocity.Y - omega * tempY) * exp;

            outputX = target.X + (changeX + tempX) * exp;
            outputY = target.Y + (changeY + tempY) * exp;

            // Prevent overshooting
            var origMinusCurrentX = originalTo.X - current.X;
            var origMinusCurrentY = originalTo.Y - current.Y;
            var outMinusOrigX = outputX - originalTo.X;
            var outMinusOrigY = outputY - originalTo.Y;

            if (origMinusCurrentX * outMinusOrigX + origMinusCurrentY * outMinusOrigY > 0)
            {
                outputX = originalTo.X;
                outputY = originalTo.Y;

                var deltaTimeInv = 1 / deltaTime;
                currentVelocity.X = (outputX - originalTo.X) * deltaTimeInv;
                currentVelocity.Y = (outputY - originalTo.Y) * deltaTimeInv;
            }

            return new Vector2(outputX, outputY);
        }
        
        public static float Lerp(float a, float b, float t) =>
            a + (b - a) * t;

        public static Color Lerp(Color a, Color b, float t)
        {
            // TODO: Remove Raylib_cs dependencies and implement SDL3 math logic as needed.
            var tinv = 1.0f - t;
            return new Color(
                (byte)(a.R * tinv + b.R * t),
                (byte)(a.G * tinv + b.G * t),
                (byte)(a.B * tinv + b.B * t),
                (byte)(a.A * tinv + b.A * t));
        }

        public static float CubicBezier(float x1, float y1, float x2, float y2, float t)
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

        public static float Clamp(float value, float min, float max) =>
            MathF.Min(max, Math.Max(min, value));

        public static float SignedAngleDelta(float angle1, float angle2)
        {
            var delta = angle2 - angle1;

            if (MathF.Abs(delta) > 180)
                delta -= 360 * MathF.Sign(delta);

            return delta;
        }

        public static float NormalizeAngle(float angle)
        {
            while (angle < 0)
                angle += 360;

            while (angle >= 360)
                angle -= 360;

            return angle;
        }

        public static float Clamp01(float value) =>
            Math.Clamp(value, 0, 1);
    }
}
