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
    public class Transition
    {
        /// <summary>
        /// Animation to play on outgoing scene
        /// </summary>
        private Animation _outgoingAnimation;

        /// <summary>
        /// Animation to play on incoming scene
        /// </summary>
        private Animation _incomingAnimation;

        /// <summary>
        /// Outgoing scene
        /// </summary>
        private Scene _outgoingScene;

        /// <summary>
        /// Incoming scene
        /// </summary>
        private Scene _incomingScene;

        /// <summary>
        /// True if the incoming scene should be rendered on top of the outgoing scene 
        /// </summary>
        private bool _incomingOnTop;

        /// <summary>
        /// True if the incoming scene should be paused during the transition
        /// </summary>
        private bool _pauseIncomingScene;

        /// <summary>
        /// True if the outgoing scene should be paused during the transition
        /// </summary>
        private bool _pauseOutgoingScene;

        /// <summary>
        /// Action to execute when the transition is complete
        /// </summary>
        private Action _onStop;

        /// <summary>
        /// Number of animations playing 
        /// </summary>
        private int _playCount;

        /// <summary>
        /// True if the transition is still playing.
        /// </summary>
        public bool IsPlaying => _playCount > 0;

        private Transition(Animation incomingAnimation, Animation outgoingAnimation, bool incomingOnTop)
        {
            _incomingAnimation = incomingAnimation;
            _outgoingAnimation = outgoingAnimation;
            _incomingOnTop = incomingOnTop;
            _pauseIncomingScene = true;
            _pauseOutgoingScene = true;
            _incomingAnimation?.OnStop(OnStop);
            _outgoingAnimation?.OnStop(OnStop);
        }

        /// <summary>
        /// Move the new scene in from right to left on top of old scene
        /// </summary>
        public static Transition MoveLeft() =>
            new Transition(
                Animation.Move(new Vector2(Window.Size.x, 0), Vector2.Zero),
                null,
                true);

        /// <summary>
        /// Move the new scene in from right to left on top of old scene
        /// </summary>
        public static Transition MoveRight() =>
            new Transition(
                Animation.Move(new Vector2(-Window.Size.x, 0), Vector2.Zero),
                null,
                true);

        /// <summary>
        /// Move the old scene to the left to reveal the new scene below it
        /// </summary>
        public static Transition RevealLeft () =>
            new Transition(
                null,
                Animation.Move(Vector2.Zero, new Vector2(-Window.Size.x, 0)),
                false);

        /// <summary>
        /// Move the new scene in from right to left on top of old scene
        /// </summary>
        public static Transition RevealRight() =>
            new Transition(
                null,
                Animation.Move(Vector2.Zero, new Vector2(Window.Size.x, 0)),
                true);

        /// <summary>
        /// Push the old scene out towards the left
        /// </summary>
        public static Transition PushLeft () => 
            new Transition(
                Animation.Move(new Vector2(Window.Size.x,0), Vector2.Zero),
                Animation.Move(Vector2.Zero, new Vector2(-Window.Size.x, 0)),
                true);

        /// <summary>
        /// Push the old scene out towards the right
        /// </summary>
        /// <returns></returns>
        public static Transition PushRight () =>
            new Transition(
                Animation.Move(new Vector2(-Window.Size.x, 0), Vector2.Zero),
                Animation.Move(Vector2.Zero, new Vector2(Window.Size.x, 0)),
                true);

        /// <summary>
        /// Fade the incoming scene in and the outgoing scene out
        /// </summary>
        /// <returns></returns>
        public static Transition CrossFade() =>
            new Transition(
                Animation.Fade(0.0f, 1.0f),
                Animation.Fade(1.0f, 0.0f),
                true);

        public Transition PauseOutgoingScene(bool paused)
        {
            _pauseOutgoingScene = paused;
            return this;
        }

        public Transition PauseIncomingScene(bool paused)
        {
            _pauseIncomingScene = paused;
            return this;
        }

        /// <summary>
        /// Set the duration of the transition
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        public Transition Duration (float duration)
        {
            _incomingAnimation?.Duration(duration);
            _outgoingAnimation?.Duration(duration);
            return this;
        }

        public Transition Easing(EaseType easeType)
        {
            _incomingAnimation?.Easing(easeType);
            _outgoingAnimation?.Easing(easeType);
            return this;
        }

        public Transition Easing(EaseType easeType, float param1)
        {
            _incomingAnimation?.Easing(easeType, param1);
            _outgoingAnimation?.Easing(easeType, param1);
            return this;
        }

        public Transition Easing(EaseType easeType, float param1, float param2)
        {
            _incomingAnimation?.Easing(easeType, param1, param2);
            _outgoingAnimation?.Easing(easeType, param1, param2);
            return this;
        }
        
        public Transition EaseInElastic(int oscillations, float springiness) => Easing(EaseType.EaseInElastic, oscillations, springiness);
        public Transition EaseInElastic() => Easing(EaseType.EaseInElastic);
        public Transition EaseInOutElastic(int oscillations, float springiness) => Easing(EaseType.EaseInOutElastic, oscillations, springiness);
        public Transition EaseInOutElastic() => Easing(EaseType.EaseInOutElastic);
        public Transition EaseOutElastic(int oscillations, float springiness) => Easing(EaseType.EaseOutElastic, oscillations, springiness);
        public Transition EaseOutlastic() => Easing(EaseType.EaseOutElastic);
        public Transition EaseInCubic() => Easing(EaseType.EaseInCubic);
        public Transition EaseOutCubic() => Easing(EaseType.EaseOutCubic);
        public Transition EaseInOutCubic() => Easing(EaseType.EaseInOutCubic);

        /// <summary>
        /// Set action to be called when the transition finishes
        /// </summary>
        public Transition OnStop(Action onStop)
        {
            _onStop = onStop;
            return this;
        }

        private void OnStop ()
        {
            _playCount--;
            if (_playCount == 0)
            {
                if (_outgoingScene != null)
                    _outgoingScene.View = null;

                if (_incomingScene != null)
                    _incomingScene.IsPaused = false;

                _onStop?.Invoke();
            }
        }

        public void Start(Scene incomingScene, Scene outgoingScene)
        {
            if (_incomingAnimation == null)
                throw new System.InvalidOperationException("Transitions cannot be started more than once");

            _outgoingScene = outgoingScene;
            _incomingScene = incomingScene;

            if (outgoingScene != null)
            {
                _outgoingAnimation?.Start(outgoingScene);
                _outgoingScene.IsPaused = _pauseOutgoingScene;
                _playCount++;
            }
            else
                _outgoingAnimation.Cancel();

            if (incomingScene != null)
            {
                _incomingAnimation?.Start(incomingScene);
                _incomingScene.IsPaused = _pauseIncomingScene;
                _playCount++;
            }
            else
                _incomingAnimation.Cancel();

            _outgoingAnimation = null;
            _incomingAnimation = null;
        }

        public void Draw (GraphicsContext gc)
        {
            if(_incomingOnTop)
            {
                _outgoingScene?.Present(gc);
                _incomingScene?.Present(gc);
            }
            else
            {
                _incomingScene?.Present(gc);
                _outgoingScene?.Present(gc);
            }
        }
    }
}
