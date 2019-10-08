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
    public abstract class PhysicsNode : Node
    {
        public static readonly Event<Collision> CollisionEnterEvent = new Event<Collision>();

        public bool IsEnabled { get; private set; }

        protected override void OnSceneChanged(Scene oldScene)
        {
            base.OnSceneChanged(oldScene);

            oldScene?.World.Unsubscribe(World.UpdateEvent, this);

            // Disable and renable physics to reset physics for the new scene
            Disable();
            Enable();
        }

        protected override void OnAnscestorChanged()
        {
            base.OnAnscestorChanged();

            // Make sure we are parented to a body.
        }

        public void Enable ()
        {
            if (IsEnabled)
                return;

            if (null == Scene || null == Scene.World)
                return;

            Scene.World.Subscribe(World.UpdateEvent, OnUpdate);

            IsEnabled = true;

            OnEnable();
        }

        public void Disable ()
        {
            if (!IsEnabled)
                return;

            Scene?.World.Unsubscribe(World.UpdateEvent, this);

            OnDisable();

            IsEnabled = false;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            Disable();
        }

        protected virtual void OnUpdate (float deltaTime)
        {            
        }

        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }
    }
}
