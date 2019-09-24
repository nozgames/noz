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

using NoZ.Graphics;

namespace NoZ
{
    public class View : ILayer
    {
        public Scene Scene { get; private set; }

        public Rect Rect { get; private set; }

        public Vector2Int Size { get; private set; }

        public int ReferenceSize { get; set; }

        public Orientation ReferenceOrientation { get; set; } = Orientation.Horizontal;

        public int SortOrder => 0;

        public void PresentScene (Scene scene, Transition transition = null)
        {
            // TODO: transition
            Scene = scene;

            Size = Window.Instance.Size;
        
            UpdateScene();
        }

        public void Frame (GraphicsContext gc)
        {
            if (null == Scene)
                return;

            Size = Window.Instance.Size;
            UpdateScene();

            Scene.Present(gc);
        }

        public void BeginLayer(GraphicsContext gc)
        {
            
        }

        public void EndLayer(GraphicsContext gc)
        {
            
        }

        public void UpdateScene ( )
        {
            if (null == Scene)
                return;

            Scene.View = this;

            if (ReferenceSize > 0)
            {
                if (ReferenceOrientation == Orientation.Horizontal)
                {
                    Scene.Scale = new Vector2(Size.x / (float)ReferenceSize);
                    Scene.Size = new Vector2Int(ReferenceSize, (int)(Size.y * (ReferenceSize / (float)Size.x)));
                }
                else
                {
                    Scene.Scale = new Vector2(Size.y / (float)ReferenceSize);
                    Scene.Size = new Vector2Int((int)(Size.x * (ReferenceSize / (float)Size.y)), ReferenceSize);
                }
            }
            else
            {
                Scene.Scale = Vector2.One;
                Scene.Size = Size;
            }

            Scene.Position = Vector2.Zero;
        }
    }
}
