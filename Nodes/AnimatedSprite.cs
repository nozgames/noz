/*
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

#if false
        /// <summary>
        /// Current animation playing playing
        /// </summary>
        public ImageAnimation Animation => _animation;
            get => _animation;
            set {
                var hadAnimation = _animation != null;

                _animation = value;
                _animationFrame = 0;
                _animationTime = 0;

                var hasAnimation = _animation != null;
                if (hasAnimation)
                {
                    _image = _animation.Frames[0];
                    UpdateActualSize();
                }

                if (hasAnimation && !hadAnimation && Scene != null)
                    Scene.Subscribe(Scene.UpdateEvent, OnAnimationUpdate);
                else if (!hasAnimation && hadAnimation && Scene != null)
                {
                    Scene.Unsubscribe(Scene.UpdateEvent, this);
                    _animationFrame = -1;
                }
            }
        }
#endif

        public float AnimationSpeed { get; set; } = 1.0f;

        public AnimatedSprite()
        {
        }

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
            base.Image = _animation.Frames[0];
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
            _animationTime += (Time.DeltaTime * AnimationSpeed);
            var timePerFrame = 1.0f / _animation.FramesPerSecond;
            var frameAdvance = (int)(_animationTime / timePerFrame);
            if (frameAdvance == 0)
                return;

            _animationTime -= frameAdvance * timePerFrame;
            _animationFrame += frameAdvance;
            if (IsLooping)
            {
                _animationFrame = (_animationFrame % _animation.Frames.Count);
                if (_animationFrame < 0)
                    _animationFrame = _animation.Frames.Count + _animationFrame;
            }
            else if (_animationFrame >= _animation.Frames.Count)
            {
                _animationFrame = -1;
                base.Image = _animation.Frames[_animation.Frames.Count - 1];
                UpdateActualSize();
                Scene.Unsubscribe(Scene.UpdateEvent, this);
                return;
            }

            base.Image = _animation.Frames[_animationFrame];
            UpdateActualSize();
        }
    }
}
