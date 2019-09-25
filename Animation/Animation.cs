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

namespace NoZ
{
    public enum AnimationUpdateMode
    {
        None,
        Update,
        LateUpdate,
        FixedUpdate
    }

    public enum EaseType
    {
        None,
        EaseInBack,
        EaseOutBack,
        EaseInOutBack,
        EaseInCubic,
        EaseOutCubic,
        EaseInOutCubic,
        EaseInBounce,
        EaseOutBounce,
        EaseInOutBounce,
        EaseInElastic,
        EaseOutElastic,
        EaseInOutElastic,
    }

    /// <summary>
    /// Plays an animation on a node
    /// </summary>
    public class Animation
    {
        [Flags]
        private enum Flags : byte
        {
            /// <summary>
            /// Indicates the animation should process the children as a sequence rather than a group
            /// </summary>
            Sequence = (1 << 0),

            /// <summary>
            /// Delta time should be unscaled
            /// </summary>
            UnscaledTime = (1 << 1),

            /// <summary>
            /// Loop the animation
            /// </summary>
            Loop = (1 << 2),

            /// <summary>
            /// Indicates the animation has been started 
            /// </summary>
            Started = (1 << 3),

            /// <summary>
            /// Indicates the animation should play itself forward, then backward before stopping
            /// </summary>
            PingPong = (1 << 4),

            /// <summary>
            /// Animation is low priority and can be stopped if necessary
            /// </summary>
            LowPriority = (1 << 5),

            /// <summary>
            /// Animation is active
            /// </summary>
            Active = (1 << 6),

            /// <summary>
            /// Animation is stopping
            /// </summary>
            Stopping = (1 << 7)
        }

        private delegate float EasingDelegate(float t, float p1, float p2);
        private delegate bool StartDelegate(Animation anim);
        private delegate void UpdateDelegate(Animation anim, float normalizedTime);

        public static bool IsPaused { get; private set; } = true;

        private AnimationUpdateMode _updateMode;
        private Vector3 _vector0;
        private Vector3 _vector1;
        private Node _node;
        private Node _target;
        private Object _object;
        private string _targetName;
        private string _key;
        private UpdateDelegate _delegate;
        private StartDelegate _startDelegate;
        private EasingDelegate _easeDelegate;
        private Action _onStop;
        private Action _onStart;
        private float _easeParam1;
        private float _easeParam2;
        private float _duration;
        private float _delay;
        private float _elapsed;
        private Animation _next;
        private Animation _prev;
        private Animation _firstChild;
        private Animation _lastChild;
        private Animation _parent;
        private Flags _flags;

        #region Static

        /// <summary>
        /// Root animation that manages all active animations
        /// </summary>
        private static Animation _root = new Animation();

        private static int _count = 0;
        private static int _countHighPriority = 0;
        private static int _countLowPriority = 0;

        public static int ActiveCount => _count;
        public static bool IsLowPriorityActive => _countLowPriority > 0;
        public static bool IsHighPriorityActive => _countHighPriority > 0;

        /// <summary>
        /// Pool of animations available for use
        /// </summary>
        private static ObjectPool<Animation> _pool = new ObjectPool<Animation>(128);

        /// <summary>
        /// Array of all availalbe easing delegates
        /// </summary>
        private readonly static EasingDelegate[] _easingDelegates;

        public static void Initialize()
        {
            _count = 0;
            _countHighPriority = 0;
            _countLowPriority = 0;
        }

        /// <summary>
        /// Stop all animations running on the given node
        /// </summary>
        public static void Stop(Node node)
        {
            if (null == node)
                return;

            Animation next;
            for (var anim = _root._firstChild; anim != null; anim = next)
            {
                next = anim._next;
                if (anim._node == node)
                    Stop(anim);
            }
        }

        /// <summary>
        /// Stop all animations on the given node that match the given key.
        /// </summary>
        public static void Stop(Node node, string key)
        {
            if (null == node)
                return;

            Animation next;
            for (var anim = _root._firstChild; anim != null; anim = next)
            {
                next = anim._next;
                if (anim._node == node && anim._key != null && anim._key == key)
                    Stop(anim);
            }
        }

        /// <summary>
        /// Stop all animations on all nodes that match the given key
        /// </summary>
        /// <param name="key"></param>
        public static void Stop(string key)
        {
            Animation next;
            for (var anim = _root._firstChild; anim != null; anim = next)
            {
                next = anim._next;
                if (anim._key != null && anim._key == key)
                    Stop(anim);
            }
        }

        /// <summary>
        /// Stop the given animation.
        /// </summary>
        /// <param name="anim"></param>
        private static void Stop(Animation anim)
        {
            if ((anim._flags & Flags.Stopping) == Flags.Stopping)
                return;

            anim._flags |= Flags.Stopping;

            // Stop all children
            while (anim._firstChild != null)
                Stop(anim._firstChild);

            anim.SetParent(null);
            anim.IsActive = false;

            var onStop = anim._onStop;

            // Free the animation
            FreeAnimation(anim);

            // Call onStop
            onStop?.Invoke();
        }

        public static void Update(AnimationUpdateMode updateMode)
        {
            var elapsedNormal = 0f;
            var elapsedUnscaled = 0f;

            switch (updateMode)
            {
                case AnimationUpdateMode.FixedUpdate:
                    elapsedNormal = Time.FixedDeltaTime;
                    elapsedUnscaled = Time.FixedUnscaledDeltaTime;
                    break;

                case AnimationUpdateMode.LateUpdate:
                case AnimationUpdateMode.Update:
                    elapsedNormal = Time.DeltaTime;
                    elapsedUnscaled = Time.UnscaledDeltaTime;
                    break;

                default:
                    throw new NotImplementedException();
            }

            Animation next = null;
            for (var anim = _root._firstChild; anim != null; anim = next)
            {
                next = anim._next;

                if (anim._updateMode != updateMode)
                    continue;

                if ((anim._flags & Flags.UnscaledTime) == Flags.UnscaledTime)
                    anim.UpdateInternal(elapsedUnscaled);
                else
                    anim.UpdateInternal(elapsedNormal);

                // If the animation is no longer active then stop it
                if (!anim.IsActive)
                    Stop(anim);
            }

            // Pause the animation system if none are running
            if (_root._firstChild == null)
                IsPaused = true;
        }

        /// <summary>
        /// Internal method used to allocate a new animation object from the pool
        /// </summary>
        /// <returns></returns>
        private static Animation AllocAnimation()
        {
            var anim = _pool.Get();
            anim._delay = 0f;
            anim._updateMode = AnimationUpdateMode.Update;
            anim._flags = 0f;
            anim._elapsed = 0f;
            anim._duration = 1f;
            anim.IsActive = false;
            return anim;
        }

        /// <summary>
        /// Internal method used to Free an animation object and return it to the pool
        /// </summary>
        /// <param name="anim"></param>
        private static void FreeAnimation(Animation anim)
        {
            anim.IsActive = false;
            anim._key = null;
            anim._onStop = null;
            anim._onStart = null;
            anim._object = null;
            anim._node = null;
            anim._target = null;
            anim._targetName = null;
            anim._easeDelegate = null;
            anim._delegate = null;
            anim._startDelegate = null;
            anim._next = null;
            anim._prev = null;
            anim._firstChild = null;
            anim._lastChild = null;
            anim._parent = null;
            _pool.Release(anim);
        }

        static Animation()
        {
            var names = Enum.GetNames(typeof(EaseType));
            _easingDelegates = new EasingDelegate[names.Length];
            for (int i = 1; i < names.Length; i++)
                _easingDelegates[i] =
                    (EasingDelegate)typeof(Animation).GetMethod(
                        names[i],
                        System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic
                        ).CreateDelegate(typeof(EasingDelegate));
        }

        #endregion  


        public Animation LowPriority() { _flags |= Flags.LowPriority; return this; }
        public Animation AddDelay(float delay) { _delay += delay; return this; }
        public Animation Delay(float delay) { _delay = delay; return this; }
        public Animation Duration(float duration) { _duration = duration; return this; }
        public Animation Target(string target) { _targetName = target; return this; }
        public Animation OnStop(Action onStop) { _onStop = onStop; return this; }
        public Animation OnStart(Action onStart) { _onStart = onStart; return this; }
        public Animation UpdateMode(AnimationUpdateMode updateMode) { _updateMode = updateMode; return this; }
        public Animation Key(string key) { _key = key; return this; }

        /// <summary>
        /// Enables or disables PingPong mode.  When PingPong mode is enabled the animation
        /// will play itself fully forward and then then in reverse before stopping.  This means that 
        /// the effective duration of the animation will be doubled. Note that everything is run in reverse 
        /// including easing modes.
        /// </summary>
        /// <param name="pingpong">True to enable PingPong mode.</param>
        public Animation PingPong(bool pingpong = true)
        {
            if (pingpong)
                _flags |= Flags.PingPong;
            else
                _flags &= ~Flags.PingPong;

            return this;
        }

        /// <summary>
        /// Set the loop state of the animation
        /// </summary>
        /// <param name="loop">True if the animation should loop</param>
        public Animation Loop(bool loop = true)
        {
            _flags = loop ? (_flags | Flags.Loop) : (_flags & ~Flags.Loop);
            return this;
        }

        /// <summary>
        /// Sets the animation to use unscaled time rather than normal scaled time
        /// </summary>
        /// <param name="unscaled">True to use unscaled time</param>
        public Animation UnscaledTime(bool unscaled = true)
        {
            _flags = unscaled ? (_flags | Flags.UnscaledTime) : (_flags & ~Flags.UnscaledTime);
            return this;
        }

        /// <summary>
        /// Adds a child to a sequence or group
        /// </summary>
        /// <param name="child">child to add</param>
        public Animation Child(Animation child)
        {
            if (child._parent != null)
                throw new ArgumentException("Child already has a parent");

            child.SetParent(this);
            return this;
        }

        public bool IsActive {
            get => (_flags & Flags.Active) == Flags.Active;
            private set {
                if (value == ((_flags & Flags.Active) == Flags.Active))
                    return;

                if (value)
                {
                    _flags |= Flags.Active;
                    _count++;
                    if (IsLowPriority)
                        _countLowPriority++;
                    else
                        _countHighPriority++;
                }
                else
                {
                    _flags &= (~Flags.Active);
                    _count--;
                    if (IsLowPriority)
                        _countLowPriority--;
                    else
                        _countHighPriority--;
                }
            }
        }

        public bool IsLooping => (_flags & Flags.Loop) == Flags.Loop;
        public bool IsPingPong => (_flags & Flags.PingPong) == Flags.PingPong;
        public bool IsSequence => (_flags & Flags.Sequence) == Flags.Sequence;
        public bool IsStarted => (_flags & Flags.Started) == Flags.Started;
        public bool IsLowPriority => (_flags & Flags.LowPriority) == Flags.LowPriority;

#if false
        public static Animation Load(TweenDefinition tweenDef)
        {
            return tweenDef.Instantiate();
        }
#endif

        public static Animation ShakePosition(Vector2 intensity)
        {
            var anim = AllocAnimation();
            anim._vector0 = intensity.ToVector3();
            anim._vector1 = new Vector3(
                Random.Range(0.0f, 100.0f),
                Random.Range(0.0f, 100.0f), 0);
            anim._delegate = ShakePositionUpdateDelegate;
            return anim;
        }

        public static Animation ShakeRotation(Vector2 intensity)
        {
            var anim = AllocAnimation();
            anim._vector0 = intensity.ToVector3();
            anim._vector1 = new Vector3(
                Random.Range(0.0f, 100.0f),
                Random.Range(0.0f, 100.0f),0);
            anim._delegate = ShakeRotationUpdateDelegate;
            return anim;
        }

        public static Animation Move(Vector2 from, Vector2 to, bool local = true)
        {
            var anim = AllocAnimation();
            anim._vector0 = from.ToVector3();
            anim._vector1 = to.ToVector3();
            anim._delegate = local ? MoveUpdateDelegate : MoveWorldUpdate;
            return anim;
        }

        public static Animation MoveTo(Vector2 to)
        {
            var anim = AllocAnimation();
            anim._vector1 = to.ToVector3();
            anim._delegate = MoveToUpdateDelegate;
            anim._startDelegate = MoveToStartDelegate;
            return anim;
        }

        public static Animation Rotate(float from, float to)
        {
            var anim = AllocAnimation();
            anim._vector0 = new Vector3(from, 0, 0);
            anim._vector1 = new Vector3(to, 0, 0);
            anim._delegate = RotateUpdateDelegate;
            return anim;
        }

        public static Animation Scale(Vector2 from, Vector2 to)
        {
            var anim = AllocAnimation();
            anim._vector0 = from.ToVector3();
            anim._vector1 = to.ToVector3();
            anim._delegate = ScaleUpdateDelegate;
            return anim;
        }

        public static Animation Scale(float from, float to) => Scale(new Vector2(from), new Vector2(to));

        public static Animation ScaleTo(Node node, Vector2 to)
        {
            var anim = AllocAnimation();
            anim._node = node;
            anim._object = node;
            anim._vector0 = node.Scale.ToVector3();
            anim._vector1 = to.ToVector3();
            anim._delegate = ScaleUpdateDelegate;
            anim._startDelegate = ScaleToStartDelegate;
            return anim;
        }

        public static Animation Activate(bool activate = true)
        {
            var anim = AllocAnimation();
            anim._delegate = ActivateUpdateDelegate;
            anim._vector0.x = activate ? 1f : 0f;
            anim._duration = 0f;
            return anim;
        }

        public static Animation Wait(float duration)
        {
            var anim = AllocAnimation();
            anim._duration = duration;
            anim._delegate = WaitUpdateDelegate;
            return anim;
        }

        public static Animation Fade(float from, float to)
        {
            var anim = AllocAnimation();
            anim._vector0.x = from;
            anim._vector1.x = to;
            anim._startDelegate = FadeStartDelegate;
            return anim;
        }

        public static Animation Color(Color from, Color to)
        {
            var anim = AllocAnimation();
            anim._vector0 = new Vector3(from.R, from.G, from.B);
            anim._vector1 = new Vector3(to.R, to.G, to.B);
            anim._startDelegate = ColorStartDelegate;
            return anim;
        }

        public static Animation Play(AudioClip clip, float volume = 1f, float pitch = 1f)
        {
            var anim = AllocAnimation();
            anim._vector0.x = volume;
            anim._vector0.y = pitch;
            anim._object = clip;
            anim._startDelegate = PlayStartDelegate;
            anim._delegate = WaitUpdateDelegate;
            return anim;
        }

        public static Animation Group()
        {
            var anim = AllocAnimation();
            anim._duration = 0f;
            return anim;
        }

        public static Animation Sequence()
        {
            var anim = AllocAnimation();
            anim._duration = 0f;
            anim._flags |= Flags.Sequence;
            return anim;
        }

        private void SetParent(Animation parent)
        {
            // Remove from the current parent
            if (_parent != null)
            {
                if (_prev != null)
                    _prev._next = _next;
                else
                    _parent._firstChild = _next;

                if (_next != null)
                    _next._prev = _prev;
                else
                    _parent._lastChild = _prev;

                _next = null;
                _prev = null;
                _parent = null;
            }

            // Add to the new parent
            if (parent != null)
            {
                _parent = parent;
                if (null == parent._firstChild)
                {
                    parent._firstChild = parent._lastChild = this;
                }
                else
                {
                    _prev = parent._lastChild;
                    parent._lastChild._next = this;
                    parent._lastChild = this;
                }
            }

        }

        /// <summary>
        /// Start the animation on the given node
        /// </summary>
        /// <param name="node">Node the animation should run on</param>
        public void Start(Node node)
        {
            // Add the animation to the root animation
            _root.Child(this);

            _node = node;

            if (!ResolveTarget(node))
                return;

            StartInternal();

            if (!IsActive)
            {
                Stop(this);
                return;
            }

            // Unpause the animation system if there are animations to run
            if (_root._firstChild != null)
                IsPaused = false; 
        }

        private bool ResolveTarget(Node node)
        {
            if (_firstChild != null)
            {
                _target = node;

                Animation next;
                for (var child = _firstChild; child != null; child = next)
                {
                    next = child._next;

                    if (!child.ResolveTarget(node))
                    {
                        child.SetParent(null);
                        FreeAnimation(child);
                    }
                }

                return (_firstChild != null);
            }

            // Resolve target
            if (!string.IsNullOrWhiteSpace(_targetName))
            {
                _target = node.FindChild(_targetName);
                if (_target == null)
                {
                    Console.WriteLine($"warning: missing target '{_targetName}'");
                    FreeAnimation(this);
                    return false;
                }
            }
            else
            {
                _target = node;
                _object = _target;
            }

            return true;
        }

        private bool StartInternal()
        {
            _elapsed = 0f;
            IsActive = true;
            _flags |= Flags.Started;

            // Force our duration on our children
            if (_duration > 0f)
                for (var child = _firstChild; child != null; child = child._next)
                    child.Duration(_duration);

            // Dont cll start if there is a delay
            if (_delay <= 0f)
            {
                // First run the start delegate if we have one
                _startDelegate?.Invoke(this);

                // If no longer active then the animation was stopped by the start method
                if (!IsActive)
                    return false;

                _onStart?.Invoke();

                UpdateInternal(0f);
            }

            return true;
        }

        private float UpdateInternal(float deltaTime)
        {
            // Delay
            if (_elapsed < _delay)
            {
                _elapsed += deltaTime;
                if (_elapsed < _delay)
                    return 0f;

                deltaTime = _elapsed - _delay;

                // Start delegate after the delay
                if (_startDelegate != null)
                {
                    _startDelegate(this);

                    if (!IsActive)
                        return deltaTime;
                }

                _onStart?.Invoke();

                // Do not run delay again if looping
                if (IsLooping)
                {
                    _elapsed -= _delay;
                    _delay = 0f;
                }
            }
            else
            {
                _elapsed += deltaTime;
            }

            // Group or sequence
            if (_firstChild != null)
            {
                if (IsSequence)
                    return UpdateSequence(deltaTime);

                return UpdateGroup(deltaTime);
            }

            // All other animations
            var elapsed = _elapsed - _delay;
            var reverse = false;
            if (elapsed >= _duration)
            {
                var done = false;
                if (IsPingPong)
                {
                    elapsed -= _duration;
                    reverse = true;
                    if (elapsed >= _duration)
                    {
                        done = true;
                    }
                }
                else
                {
                    done = true;
                }

                if (done)
                {
                    if (IsLooping)
                    {
                        _elapsed = elapsed % _duration;
                        reverse = false;
                        elapsed = _elapsed;
                        deltaTime = 0f;
                    }
                    else
                    {
                        deltaTime = elapsed - _duration;
                        elapsed = _duration;
                        IsActive = false;
                    }
                }
                else
                {
                    deltaTime = 0f;
                }
            }
            else
            {
                deltaTime = 0f;
            }

            float t = 0f;
            if (_duration > 0.0f)
            {
                t = elapsed / _duration;
                if (_easeDelegate != null)
                    t = _easeDelegate(t, _easeParam1, _easeParam2);

                if (reverse)
                    t = 1f - t;
            }

            // Update delegate
            _delegate(this, t);

            return deltaTime;
        }

        private float UpdateGroup(float deltaTime)
        {
            // Advance all children
            Animation next;
            var done = true;
            var remainingTime = deltaTime;
            for (var child = _firstChild; child != null; child = next)
            {
                next = child._next;

                // Start the child if not yet started.
                if (!child.IsStarted)
                {
                    if (!child.StartInternal())
                    {
                        Stop(child);
                        continue;
                    }
                }

                // This will be true on looping groups for the children who 
                // have already finished.
                if (!child.IsActive)
                {
                    if (!child.IsLooping && !IsLooping)
                        Stop(child);
                    continue;
                }

                // Advance the child
                remainingTime = MathEx.Min(remainingTime, child.UpdateInternal(deltaTime));

                done &= !child.IsActive;

                // As long as the group isnt going to loop we can just stop
                // the child as soon as its done.
                if (!child.IsActive && !IsLooping)
                    Stop(child);
            }

            if (_firstChild == null)
                IsActive = false;
            else if (done && IsLooping)
            {
                _elapsed = 0f;

                // Start the children over
                for (var child = _firstChild; child != null; child = next)
                {
                    next = child._next;
                    if (!child.StartInternal())
                        Stop(child);
                }

                // Recursively call ourself to process the rest of the time
                if (remainingTime > 0)
                    return UpdateInternal(remainingTime);
            }

            return remainingTime;
        }

        private float UpdateSequence(float deltaTime)
        {
            while (_firstChild != null)
            {
                var child = _firstChild;
                if (!child.IsActive && !child.StartInternal())
                {
                    Stop(child);
                    continue;
                }

                deltaTime = child.UpdateInternal(deltaTime);

                if (!child.IsActive)
                {
                    if (IsLooping)
                    {
                        // Move the child to the end of the list
                        child.SetParent(null);
                        child.SetParent(this);
                    }
                    else
                    {
                        Stop(child);
                    }
                }

                if (deltaTime <= 0f)
                    break;
            }

            IsActive = _firstChild != null;

            return deltaTime;
        }

#region Easing

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

#endregion

#region Start Delegates

        private static bool ColorStart(Animation anim)
        {
            anim._object = anim._target as Sprite;
            if (null != anim._object)
            {
                anim._delegate = ColorSpriteUpdate;
                return true;
            }

            Console.WriteLine($"warning: target has no color property to animate");
            return false;
        }

        private static bool MoveToStart(Animation anim)
        {
            anim._object = anim._target;
            anim._vector0 = (anim._object as Node).Position.ToVector3();
            return true;
        }


        private static bool ScaleToStart(Animation anim)
        {
            anim._vector0 = (anim._object as Node).Scale.ToVector3();
            return true;
        }

        private static bool FadeStart(Animation anim)
        {
            anim._object = anim._target as Sprite;
            if (null != anim._object)
            {
                anim._delegate = FadeUpdateSpriteDelegate;
                return true;
            }

            anim._object = anim._target as Label;
            if (null != anim._object)
            {
                anim._delegate = FadeUpdateLabelDelegate;
                return true;
            }

            return false;
        }

        private static bool PlayStart(Animation anim)
        {
            if (anim._object.GetType() == typeof(AudioClip))
            {
                (anim._object as AudioClip).Play(); //  tween._vector0.x, tween._vector0.y);
            }
            return true;
        }

        private static readonly StartDelegate PlayStartDelegate = PlayStart;
        private static readonly StartDelegate ColorStartDelegate = ColorStart;
        private static readonly StartDelegate FadeStartDelegate = FadeStart;
        private static readonly StartDelegate MoveToStartDelegate = MoveToStart;
        private static readonly StartDelegate ScaleToStartDelegate = ScaleToStart;

#endregion

#region Update Delegates

        private static void ActivateUpdate(Animation anim, float t)
        {
#if false
            var active = tween._vector0.x > 0;
            if (active != tween._target.activeSelf)
                tween._target.SetActive(active);
#endif
        }

        private static void ShakePositionUpdate(Animation anim, float t)
        {
            (anim._object as Node).Position =
                new Vector2(
                    anim._vector0.x * (MathEx.PerlinNoise(anim._vector1.x, t * 20f) - 0.5f) * 2.0f,
                    anim._vector0.y * (MathEx.PerlinNoise(anim._vector1.y, t * 20f) - 0.5f) * 2.0f
                    ) * (1f - t);
        }

        private static void ShakeRotationUpdate(Animation anim, float t)
        {
            (anim._object as Node).Rotation =
                anim._vector0.x * (MathEx.PerlinNoise(anim._vector1.x, t * 20f) - 0.5f) * 2.0f * (1f - t);
        }

        private static void MoveUpdate(Animation anim, float t)
        {
            (anim._object as Node).Position = (anim._vector0 * (1 - t) + anim._vector1 * t).ToVector2();
        }

        private static void MoveWorldUpdate(Animation anim, float t)
        {
            (anim._object as Node).Position = (anim._vector0 * (1 - t) + anim._vector1 * t).ToVector2();
        }

        private static void MoveToUpdate(Animation anim, float t)
        {
            (anim._object as Node).Position = (anim._vector0 * (1 - t) + anim._vector1 * t).ToVector2();
        }

        private static void ScaleUpdate(Animation anim, float t)
        {
            (anim._object as Node).Scale = (anim._vector0 * (1 - t) + anim._vector1 * t).ToVector2();
        }

        private static void RotateUpdate(Animation anim, float t)
        {
            (anim._object as Node).Rotation = anim._vector0.x * (1 - t) + anim._vector1.x * t;
        }

#if false
        private static void FadeUpdateCanvasGroup(Animation anim, float t)
        {
            (anim._object as CanvasGroup).alpha = anim._vector0.x * (1f - t) + anim._vector1.x * t;
        }
#endif

        private static void FadeUpdateSprite(Animation anim, float t)
        {
            var sprite = (anim._object as Sprite);
            sprite.Color = NoZ.Color.FromRgba(sprite.Color, (anim._vector0.x * (1f - t) + anim._vector1.x * t));
        }

        private static void FadeUpdateLabel(Animation anim, float t)
        {
            var label = (anim._object as Label);
            label.Color = NoZ.Color.LerpAlpha(label.Color, anim._vector0.x, anim._vector1.x, t);
        }

        private static void ColorSpriteUpdate(Animation anim, float t)
        {
            var sprite = (anim._object as Sprite);
            var color = anim._vector0 * (1f - t) + anim._vector1 * t;
            sprite.Color = NoZ.Color.FromRgba(color.x, color.y, color.z, sprite.Color.A / 255.0f);
        }

        private static void WaitUpdate(Animation anim, float t) { }

        private static readonly UpdateDelegate ActivateUpdateDelegate = ActivateUpdate;
        private static readonly UpdateDelegate MoveUpdateDelegate = MoveUpdate;
        private static readonly UpdateDelegate MoveWorldUpdateDelegate = MoveWorldUpdate;
        private static readonly UpdateDelegate MoveToUpdateDelegate = MoveToUpdate;
        private static readonly UpdateDelegate ScaleUpdateDelegate = ScaleUpdate;
        private static readonly UpdateDelegate RotateUpdateDelegate = RotateUpdate;
#if false
        private static readonly UpdateDelegate FadeUpdateCanvasGroupDelegate = FadeUpdateCanvasGroup;
#endif
        private static readonly UpdateDelegate FadeUpdateSpriteDelegate = FadeUpdateSprite;
        private static readonly UpdateDelegate FadeUpdateLabelDelegate = FadeUpdateLabel;
        private static readonly UpdateDelegate WaitUpdateDelegate = WaitUpdate;
        private static readonly UpdateDelegate ShakePositionUpdateDelegate = ShakePositionUpdate;
        private static readonly UpdateDelegate ShakeRotationUpdateDelegate = ShakeRotationUpdate;

#endregion
    }
}
