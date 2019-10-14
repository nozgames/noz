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
    public class View 
    {
        private bool _visible;
        private Transition _transition;

        public Scene Scene { get; private set; }

        public Rect Rect { get; private set; }

        public Vector2Int Size { get; private set; }

        public bool IsVisible {
            get => _visible;
            set {
                if (_visible == value)
                    return;

                _visible = value;
                if (_visible && Scene != null)
                {
                    UpdateScene();
                    Scene.InvalidateRect();
                    Scene.InvalidateTransform();
                    Scene.UpdateRect();
                    Scene.UpdateTransform();
                }
            }
        }

        public void PresentScene (Scene scene, Transition transition = null)
        {
            if (scene == Scene)
                return;

            var old = Scene;

            Scene = scene;

            if(scene != null)
                scene.View = this;

            // Apply a transition.
            if(transition != null)
            {
                _transition = transition;
                transition.Start(scene, old);
                return;
            } 
            else if (old != null)
            {
                old.View = null;
            }

            if (Scene == null)
                return;

            Size = Window.Size;
            Scene.IsPaused = false;
        
            UpdateScene();
        }

        public void Update ()
        {
            if(_transition != null && !_transition.IsPlaying)
            {
                _transition = null;

                if(Scene != null)
                    Scene.IsPaused = false;
            }

            if (null == Scene)
                return;

            Size = Window.Size;
            UpdateScene();
            Scene.Update();
        }

        public void Draw (GraphicsContext gc)
        {
            if (null != _transition)
                _transition.Draw(gc);
            else if(null != Scene)
                Scene.Present(gc);
        }

        public void UpdateScene ( )
        {
            if (null == Scene)
                return;

            if (Window.ReferenceSize > 0)
            {
                if (Window.ReferenceOrientation == Orientation.Horizontal)
                {
                    Scene.Scale = new Vector2(Size.x / (float)Window.ReferenceSize);
                    Scene.Size = new Vector2Int(Window.ReferenceSize, (int)(Size.y * (Window.ReferenceSize / (float)Size.x)));
                }
                else
                {
                    Scene.Scale = new Vector2(Size.y / (float)Window.ReferenceSize);
                    Scene.Size = new Vector2Int((int)(Size.x * (Window.ReferenceSize / (float)Size.y)), Window.ReferenceSize);
                }
            }
            else
            {
                Scene.Scale = Vector2.One;
                Scene.Size = Size;
            }

            //Scene.Position = Vector2.Zero;
        }
    }
}
