﻿/*
  NozEngine Library

  Copyright(c) 2015 NoZ Games, LLC

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
    public class AnimatedSprite : Sprite
    {
        /// <summary>
        /// Current animation
        /// </summary>
        private ImageAnimation _animation;

        /// <summary>
        /// Current frame within the animation
        /// </summary>
        private int _animationFrame = -1;

        /// <summary>
        /// Accumulated animation time
        /// </summary>
        private float _animationTime;

        /// <summary>
        /// True if an animation is playing
        /// </summary>
        public bool IsPlaying => _animationFrame >= 0;

        /// <summary>
        /// True if the playing animation is looping
        /// </summary>
        public bool IsLooping { get; private set; } = false;

        /// <summary>
        /// Action called when an animation frame contains an event
        /// </summary>
        public Action<string> OnFrameEvent { get; set; }

        /// <summary>
        /// Image used to render the rectangle.  If no image is given a solid color rectangle 
        /// will be rendered instead by using a solid white texture.
        /// </summary>
        public override Image Image {
            get => base.Image;
            set {
                Stop();

                base.Image = value;
            }
        }

        /// <summary>
        /// Current animation playing playing
        /// </summary>
        public ImageAnimation Animation => _animation;

        public float AnimationSpeed { get; set; } = 1.0f;

        public AnimatedSprite() { }

        public AnimatedSprite(Image image) : base(image) { }

        public AnimatedSprite(ImageAnimation animation) => Play(animation);

        public AnimatedSprite(ImageAnimation animation, bool loop) => Play(animation, loop);

        public AnimatedSprite(ImageAnimation animation, bool loop, float animationSpeed) => Play(animation, loop, animationSpeed);

        public void Play(ImageAnimation animation) => Play(animation, false, 1.0f);

        public void Play(ImageAnimation animation, bool loop) => Play(animation, loop, 1.0f);

        public void Play(ImageAnimation animation, bool loop, float animationSpeed)
        {
            IsLooping = loop;
            AnimationSpeed = animationSpeed;
            _animation = animation;
            if (!IsPlaying && Scene != null)
                Scene.Subscribe(Scene.UpdateEvent, Step);

            _animationFrame = 0;
            _animationTime = 0.0f;
            base.Image = _animation.Frames[0].Image;
        }

        /// <summary>
        /// Stop the current animation from playing.  The last frame of animation 
        /// displayed for the AnimatedSprite will be the frame that is displayed 
        /// after the animation stops.
        /// </summary>
        public void Stop ( )
        {
            if (!IsPlaying)
                return;

            // Remove any playing animation
            _animation = null;
            _animationFrame = -1;

            Scene.Unsubscribe(Scene.UpdateEvent, this);
        }

        /// <summary>
        /// Handle events when sprite enters the scene
        /// </summary>
        protected override void OnEnterScene(Scene scene)
        {
            base.OnEnterScene(scene);

            if (IsPlaying)
                Scene.Subscribe(Scene.UpdateEvent, Step);
        }

        /// <summary>
        /// Handle events when sprite leaves the scene
        /// </summary>
        protected override void OnLeaveScene(Scene leaving)
        {
            base.OnLeaveScene(leaving);

            if (IsPlaying)
                Scene.Unsubscribe(Scene.UpdateEvent, this);
        }

        /// <summary>
        /// Handle a single step of the animation
        /// </summary>
        private void Step()
        {
            // Determine where in the animation we are
            if(IsLooping)
            {
                _animationTime = (_animationTime + Time.DeltaTime * AnimationSpeed) % _animation.Duration;
            }
            else 
            {
                _animationTime += (Time.DeltaTime * AnimationSpeed);
                if (_animationTime > _animation.Duration)
                {
                    _animationTime = _animation.Duration;
                    _animationFrame = -1;
                    base.Image = _animation.Frames[_animation.Frames.Length - 1].Image;
                    Scene.Unsubscribe(Scene.UpdateEvent, this);
                    return;
                }                
            }

            var frame = _animation.GetFrameIndex(_animationTime, _animationFrame);
            if(_animationFrame != frame)
            {
                FireEvents(_animationFrame, frame);

                _animationFrame = frame;
                base.Image = _animation.Frames[_animationFrame].Image;
            }                
        }

        /// <summary>
        /// Fire all frame events that occurred
        /// </summary>
        private void FireEvents (int oldFrame, int newFrame)
        {
            if (null == OnFrameEvent)
                return;

            for(int i=oldFrame; i != newFrame; i = (i+1) % _animation.Frames.Length)
            {
                var events = _animation.Frames[i].Events;
                if (null == events)
                    continue;

                for (int e = 0; e < events.Length; e++)
                    OnFrameEvent(events[e]);
            }
        }
    }
}
