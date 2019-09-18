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
    public class Game
    {
        public delegate void RunDelegate();

        public static Game Instance {
            get; private set;
        }

        private IAudioDriver _audioDriver;
        private IGraphicsDriver _graphicsDriver;
        private Window _window;

        public static IAudioDriver AudioDriver { get { return Instance._audioDriver; } }
        public static IGraphicsDriver GraphicsDriver { get { return Instance._graphicsDriver; } }

        public Game (Window window, IGraphicsDriver graphicsDriver, IAudioDriver audioDriver)
        {
            _window = window;
            _graphicsDriver = graphicsDriver;
            _audioDriver = audioDriver;
            Instance = this;
        }

        public void Run ( )
        {

        }

        public void Frame ( )
        {
            _window.BeginFrame();
            _graphicsDriver.BeginFrame();

            var context = _graphicsDriver.CreateContext();
            context.Begin(new Vector2Int(800, 600), Color.Red);
            context.End();

            _graphicsDriver.EndFrame();
            _window.EndFrame();
        }

        /// <summary>
        /// Transitions from the current scene to the given scene
        /// </summary>
        /// <param name="scene"></param>
        public void PresentScene (Scene scene, Transition transition=null)
        {

        }
    }
}
