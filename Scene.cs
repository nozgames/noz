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
    public class Scene : Node, ILayer
    {
        public static readonly Event<KeyCode> KeyDownEvent = new Event<KeyCode>();
        public static readonly Event<KeyCode> KeyUpEvent = new Event<KeyCode>();
        public static readonly Event UpdateEvent = new Event();
        public static readonly Event<View> ViewChangedEvent = new Event<View>();

        private DrawList _drawList;
        private Vector2Int _size;
        private Camera _camera;
        private bool _paused = true;
        private Matrix3 _windowToScene;
        private Matrix3 _sceneToWindow;
        private World _world;
        private View _view;

        public Node Cursor { get; set; }

        public bool IsPaused {
            get => _paused;
            set {
                if (_paused == value)
                    return;

                _paused = value;

                if (_paused)
                {
                    Input.KeyDownEvent.UnsubscribeAll(this);
                    OnPause();
                }
                else
                {
                    Input.KeyDownEvent.Subscribe(OnKeyDown);
                    Input.KeyDownEvent.Subscribe(OnKeyUp);
                    OnResume();
                }
            }
        }

        public bool HasWorld => _world != null;

        /// <summary>
        /// Physics world associated with the scene
        /// </summary>
        public World World {
            get {
                if (_world == null)
                    _world = new World();

                return _world;
            }
        }

        public View View {
            get => _view;
            internal set {
                if (_view == value)
                    return;

                _view = value;
                OnViewChanged(_view);
            }
        }

        public override bool DoesTransformAffectChildren => false;

        public Vector2Int Size {
            get => _size;
            set {
                if (_size != value) {
                    _size = value;
                    InvalidateRect();
                }
            }
        }

        public Camera Camera {
            get => _camera;
            set {
                if (value == _camera)
                    return;

                _camera = value;
            }
        }

        public int SortOrder { get; private set; }

        public Scene ()
        {
            _drawList = new DrawList();
        }

        public void Present (GraphicsContext gc)
        {
            if (Camera != null)
            {
                // Generate the camera matrix
                var mat = Matrix3.Translate(Window.Size.ToVector2() * 0.5f);
                mat = Matrix3.Multiply(LocalToWorld, mat);


                var mat2 = Matrix3.Translate(-Camera.Parent.LocalToWorld.MultiplyVector(Camera.Position));
                mat2 = Matrix3.Multiply(mat2, Matrix3.Scale(Camera.Scale));
                mat2 = Matrix3.Multiply(mat2, Matrix3.Rotate(Camera.Rotation * MathEx.Deg2Rad));
                mat = Matrix3.Multiply(mat2, mat);

                _sceneToWindow = mat;
                _windowToScene = mat.Inverse();

                gc.PushMatrix(mat);
            }
            else
            {
                gc.PushMatrix(LocalToWorld);

                _windowToScene = WorldToLocal;
            }

            _drawList.Build(this);
            _drawList.Draw(gc);
            _drawList.Clear();

            _world?.DrawDebug(gc);

            gc.PopMatrix();
        }

        public void BeginLayer(GraphicsContext gc)
        {
            
        }

        public void EndLayer(GraphicsContext gc)
        {
            
        }

        public void Update ()
        {
            if (IsPaused)
                return;

            OnUpdate();

            Broadcast(UpdateEvent);
        }


        /// <summary>
        /// Returns the top most interactive node at the given position within the scene
        /// </summary>
        /// <param name="position">Position to test at</param>
        /// <param name="type">Optional filter of node type</param>
        /// <returns>Top most node under the given mouse position</returns>
        private static Node GetNodeAtPointInternal(Node root, in Vector2 position, Type type = null)
        {
            var result = root.HitTest(position);
            if (result == HitTestResult.Ignore)
                return null;

            for (int i = root.ChildCount - 1; i >= 0; i--)
            {
                Node hit = GetNodeAtPointInternal(root.GetChildAt(i), position, type);
                if (null != hit)
                    return hit;
            }

            if (result == HitTestResult.Hit)
            {
                if (type != null)
                    while (root != null && !(type.IsAssignableFrom(root.GetType())))
                        root = root.Parent;

                return root;
            }

            return null;
        }

        /// <summary>
        /// Returns the top most interactive node at the given position within the scene
        /// </summary>
        /// <param name="position">Position to test at</param>
        /// <param name="type">Optional filter of node type</param>
        /// <returns>Top most node under the given mouse position</returns>
        public Node GetNodeAtPoint(Node root, in Vector2 position, Type type = null)
        {
            if (root == null)
            {
                for (int i = ChildCount - 1; i >= 0; i--)
                {
                    var hit = GetNodeAtPointInternal(GetChildAt(i), position, type);
                    if (null != hit)
                        return hit;
                }

                return null;
            }
            else if (root.Scene != this)
                return null;

            return GetNodeAtPointInternal(root, position, type);
        }

        public T GetNodeAtPoint<T>(Node root, in Vector2 worldPosition) where T : Node
        {
            return GetNodeAtPoint(root, worldPosition, typeof(T)) as T;
        }

        protected virtual void OnUpdate() { }
        protected virtual void OnPause() { }
        protected virtual void OnResume() { }

        public Matrix3 WindowToScene => _windowToScene;

        public Matrix3 SceneToWindow => _sceneToWindow;

        protected virtual void OnKeyDown(KeyCode keyCode) => Broadcast(KeyDownEvent, keyCode);

        protected virtual void OnKeyUp(KeyCode keyCode) => Broadcast(KeyUpEvent, keyCode);

        protected override Vector2 MeasureOverride(in Vector2 available) => _size.ToVector2();

        protected override Vector2 GetPivot() => Vector2.Zero;

        /// <summary>
        /// Called when the scenes view changes.
        /// </summary>
        protected internal virtual void OnViewChanged(View view) {
            Broadcast(ViewChangedEvent, view);
        }

        protected internal override void OnMouseOver(MouseOverEvent e)
        {
            base.OnMouseOver(e);
            e.Cursor = Cursor;
        }
    }
}
