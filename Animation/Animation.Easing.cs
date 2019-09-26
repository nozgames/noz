/*
  NoZ Game Engine

  Copyright(c) 2019 NoZ Games, LLC

  Permission is hereby granted, free of charge, to any person obtaining a copy
  of this software and associated documentation files(the "Software"), to deal
  in the Software without restriction, including without limitation the rights
  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
  copies of the Software, and to permit persons to whom the Software is
  furnished to do so, subject to the following conditions :

  The above copyright notice and this permission notice shall be included in all
  copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
  SOFTWARE.
*/

namespace NoZ
{
    public partial class Animation
    {
        public Animation EaseInBack(float amplitude = 1f) => Easing(EaseType.EaseInBack, amplitude, 0f);
        public Animation EaseOutBack(float amplitude = 1f) => Easing(EaseType.EaseOutBack, amplitude, 0f);
        public Animation EaseInOutBack(float amplitude = 1f) => Easing(EaseType.EaseInOutBack, amplitude, 0f);

        public Animation EaseInElastic(int oscillations, float springiness) => Easing(EaseType.EaseInElastic, oscillations, springiness);
        public Animation EaseInElastic() => Easing(EaseType.EaseInElastic);
        public Animation EaseInOutElastic(int oscillations, float springiness) => Easing(EaseType.EaseInOutElastic, oscillations, springiness);
        public Animation EaseInOutElastic() => Easing(EaseType.EaseInOutElastic);
        public Animation EaseOutElastic(int oscillations, float springiness) => Easing(EaseType.EaseOutElastic, oscillations, springiness);
        public Animation EaseOutlastic() => Easing(EaseType.EaseOutElastic);
        public Animation EaseInCubic() => Easing(EaseType.EaseInCubic);
        public Animation EaseOutCubic() => Easing(EaseType.EaseOutCubic);
        public Animation EaseInOutCubic() => Easing(EaseType.EaseInOutCubic);

        public Animation Easing(EaseType easeType)
        {
            _easeDelegate = _easingDelegates[(int)easeType];

            switch (easeType)
            {
                case EaseType.EaseInBounce:
                case EaseType.EaseInOutBounce:
                case EaseType.EaseOutBounce:
                    _easeParam1 = 3f;
                    _easeParam2 = 2f;
                    break;

                case EaseType.EaseInElastic:
                case EaseType.EaseOutElastic:
                case EaseType.EaseInOutElastic:
                    _easeParam1 = 3f;
                    _easeParam2 = 3f;
                    break;

                default:
                    _easeParam1 = 0f;
                    _easeParam2 = 0f;
                    break;
            }

            return this;
        }

        public Animation Easing(EaseType easeType, float param1)
        {
            Easing(easeType);
            _easeParam1 = param1;
            return this;
        }

        public Animation Easing(EaseType easeType, float param1, float param2)
        {
            _easeDelegate = _easingDelegates[(int)easeType];
            _easeParam1 = param1;
            _easeParam2 = param2;
            return this;
        }

        private static float EaseOut(float t, float p1, float p2, EaseType easeType) => 1f - _easingDelegates[(int)easeType](1f - t, p1, p2);
        private static float EaseInOut(float t, float p1, float p2, EaseType easeType)
        {
            var easeIn = _easingDelegates[(int)easeType];
            return (t < 0.5f) ?
                easeIn(t * 2f, p1, p2) * 0.5f :
                (1f - easeIn((1f - t) * 2f, p1, p2)) * 0.5f + 0.5f;
        }

        private static float EaseInCubic(float t, float p1, float p2) => t * t * t;
        private static float EaseOutCubic(float t, float p1, float p2) => EaseOut(t, p1, p2, EaseType.EaseInCubic);
        private static float EaseInOutCubic(float t, float p1, float p2) => EaseInOut(t, p1, p2, EaseType.EaseInCubic);

        private static float EaseInBack(float t, float p1, float p2)
        {
            return MathEx.Pow(t, 3f) - t * MathEx.Max(0f, p1) * MathEx.Sin(MathEx.PI * t);
        }
        private static float EaseOutBack(float t, float p1, float p2) => EaseOut(t, p1, p2, EaseType.EaseInBack);
        private static float EaseInOutBack(float t, float p1, float p2) => EaseInOut(t, p1, p2, EaseType.EaseInBack);

        private static float EaseInBounce(float t, float p1, float p2)
        {
            var Bounces = p1;
            var Bounciness = p2;

            var pow = MathEx.Pow(Bounciness, Bounces);
            var invBounciness = 1f - Bounciness;

            var sum_units = (1f - pow) / invBounciness + pow * 0.5f;
            var unit_at_t = t * sum_units;

            var bounce_at_t = MathEx.Log(-unit_at_t * invBounciness + 1f, Bounciness);
            var start = MathEx.Floor(bounce_at_t);
            var end = start + 1f;

            var div = 1f / (invBounciness * sum_units);
            var start_time = (1f - MathEx.Pow(Bounciness, start)) * div;
            var end_time = (1f - MathEx.Pow(Bounciness, end)) * div;

            var mid_time = (start_time + end_time) * 0.5f;
            var peak_time = t - mid_time;
            var radius = mid_time - start_time;
            var amplitude = MathEx.Pow(1f / Bounciness, Bounces - start);

            return (-amplitude / (radius * radius)) * (peak_time - radius) * (peak_time + radius);
        }
        private static float EaseOutBounce(float t, float p1, float p2) => EaseOut(t, p1, p2, EaseType.EaseInBounce);
        private static float EaseInOutBounce(float t, float p1, float p2) => EaseInOut(t, p1, p2, EaseType.EaseInBounce);

        private static float EaseInElastic(float t, float oscillations, float springiness)
        {
            oscillations = MathEx.Max(0, (int)oscillations);
            springiness = MathEx.Max(0f, springiness);

            float expo;
            if (springiness == 0f)
                expo = t;
            else
                expo = (MathEx.Exp(springiness * t) - 1f) / (MathEx.Exp(springiness) - 1f);

            return expo * (MathEx.Sin((MathEx.PI * 2f * oscillations + MathEx.PI * 0.5f) * t));
        }
        private static float EaseOutElastic(float t, float p1, float p2) => EaseOut(t, p1, p2, EaseType.EaseInElastic);
        private static float EaseInOutElastic(float t, float p1, float p2) => EaseInOut(t, p1, p2, EaseType.EaseInElastic);
    }
}
