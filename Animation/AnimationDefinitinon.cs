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

using System;
using System.Collections.Generic;

namespace NoZ
{
#if false
    public class AnimationDefinition 
    {
        public enum TweenType
        {
            Move,
            Scale,
            Fade,
            ShakePosition,
            ShakeRotation,
            Color,
            Play,
            Wait,
            Activate
        }

        private class SerializedTween
        {
            public TweenType Type = TweenType.Move;
            public string Target = null;
            public float Duration = 1f;
            public float Delay = 0f;
            public bool IsPingPong = false;
            public bool IsLooping = false;
            public bool IsLowPriority = false;
            public Vector3 From = Vector3.zero;
            public Vector3 To = Vector3.zero;
            public EaseType Easing = EaseType.None;
            public float EasingParam1 = 0f;
            public float EasingParam2 = 0f;
            public UnityEngine.Object Object = null;
        }

        [SerializeField]
        public bool IsSequence = false;

        [SerializeField]
        private bool DurationOverride = false;

        [SerializeField]
        private float Duration = 1f;

        [SerializeField]
        private float Delay = 0f;

        [SerializeField]
        private bool IsLooping = false;

        [SerializeField]
        private bool IsPingPong = false;

        [SerializeField]
        private SerializedTween[] _tweens = null;

        [SerializeField]
        private string _target = null;

        [SerializeField]
        private bool IsLowPriority = false;

        public Animation Instantiate()
        {
            if (null == _tweens)
                return null;

            Animation tween = IsSequence ? Animation.Sequence() : Animation.Group();

            for (int i = 0; i < _tweens.Length; i++)
            {
                Animation child = null;

                switch (_tweens[i].Type)
                {
                    case TweenType.Move: child = Animation.Move(_tweens[i].From, _tweens[i].To); break;
                    case TweenType.Fade:
                        child = Animation.Fade(_tweens[i].From.x, _tweens[i].To.x);
                        break;
                    case TweenType.Color:
                        child = Animation.Color(
                            new Color(_tweens[i].From.x, _tweens[i].From.y, _tweens[i].From.z),
                            new Color(_tweens[i].To.x, _tweens[i].To.y, _tweens[i].To.z)
                        );
                        break;
                    case TweenType.Scale:
                        child = Animation.Scale(_tweens[i].From.x, _tweens[i].To.x);
                        break;
                    case TweenType.ShakePosition:
                        child = Animation.ShakePosition(_tweens[i].From);
                        break;
                    case TweenType.ShakeRotation:
                        child = Animation.ShakeRotation(_tweens[i].From);
                        break;
                    case TweenType.Play:
                        child = Animation.Play(_tweens[i].Object as AudioClip, _tweens[i].From.x, _tweens[i].From.y);
                        break;
                    case TweenType.Wait:
                        child = Animation.Wait(0f);
                        break;
                    case TweenType.Activate:
                        child = Animation.Activate(_tweens[i].From.x > 0);
                        break;
                }

                if (!string.IsNullOrWhiteSpace(_tweens[i].Target))
                    child.Target(_tweens[i].Target);

                child.Easing(_tweens[i].Easing, _tweens[i].EasingParam1, _tweens[i].EasingParam2);
                child.PingPong(_tweens[i].IsPingPong);
                child.Loop(_tweens[i].IsLooping);
                child.Duration(_tweens[i].Duration);
                child.Delay(_tweens[i].Delay);

                if (_tweens[i].IsLowPriority)
                    child.LowPriority();

                tween.Child(child);
            }

            if (!string.IsNullOrWhiteSpace(_target))
                tween.Target(_target);

            tween.Loop(IsLooping);
            tween.PingPong(IsPingPong);

            if (IsLowPriority)
                tween.LowPriority();

            if (DurationOverride)
                tween.Duration(Duration);

            tween.Delay(Delay);

            return tween;
        }

        public void AddTween(TweenType type)
        {
            if (null == _tweens)
            {
                _tweens = new SerializedTween[1];
            }
            else
            {
                Array.Resize<SerializedTween>(ref _tweens, _tweens.Length + 1);
            }

            var tween = new SerializedTween
            {
                Type = type
            };

            switch (type)
            {
                case TweenType.Color:
                    tween.From = tween.To = Vector3.one;
                    break;

                case TweenType.Play:
                    tween.From = Vector3.one;
                    break;
            }

            _tweens[_tweens.Length - 1] = tween;
        }

        public void RemoveTween(int index)
        {
            if (null == _tweens)
                return;

            var list = new List<SerializedTween>(_tweens);
            list.RemoveAt(index);
            _tweens = list.ToArray();
        }
    }
#endif
}
