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
    public class Camera : Node, ILayer
    {
        // TODO: GetVisibleNodes

        // TODO: Contains

        public override bool DoesTransformAffectChildren => false;

        public int SortOrder => 10000;

        protected override void OnVisibleChanged(bool visible)
        {
            base.OnVisibleChanged(visible);

            // If this is the first visible camera then set it to active
            if(Scene != null)
                Scene.Camera = this;
        }

        protected override void OnSceneChanged(Scene prevScene)
        {
            // Automatically set the camera as the active camera if it is the first one in the scene
            if (Scene != null && Scene.Camera == null)
                Scene.Camera = this;            
        }

        public void BeginLayer(GraphicsContext gc)
        {
            // Render all children of the camera as if they were children of the scene
            gc.PushMatrix(Scene.LocalToSceneMatrix);
        }

        public void EndLayer(GraphicsContext gc)
        {
            gc.PopMatrix();
        }
    }
}
