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
using System.Collections.Generic;

namespace NoZ
{
    public enum NodeVisibility
    {
        /// <summary>
        /// Normally visible
        /// </summary>
        Visible,

        /// <summary>
        /// Occupies space in the layout, but is not visible
        /// </summary>
        Hidden,

        /// <summary>
        /// Not visible and does not occupy any space in the layout.
        /// </summary>
        Collapsed
    }

    public enum HitTestResult
    {
        /// <summary>
        /// Indicates that the node was not hit 
        /// </summary>
        NotHit,

        /// <summary>
        /// Indicates that the node was hit
        /// </summary>
        Hit,

        /// <summary>
        /// Indicates that this node and all of its descendents should be ignored for hit testing.
        /// </summary>
        Ignore
    }

    public class Node : Object
    {
        public static readonly Event<Node> DestroyEvent = new Event<Node>();

        private enum Flags
        {
            /// <summary>
            /// Node is currently visible in the scene
            /// </summary>
            Visible = (1 << 0),

            /// <summary>
            /// Node has been destroyed
            /// </summary>
            Destroyed = (1 << 1),

            /// <summary>
            /// Node will automatically destroy itself if not parented by 
            /// the end of the frame.
            /// </summary>
            AutoDestroy = (1 << 2),

            /// <summary>
            /// Rect value within the node is dirty
            /// </summary>
            RectDirty = (1 << 3),

            /// <summary>
            /// Transform within node is dirty and must be recalculatd if requested.
            /// </summary>
            TransformDirty = (1 << 4),

            /// <summary>
            /// WorldToLocal matrix is dirty and needs to be recalculated
            /// </summary>
            WorldToLocalDirty = (1 << 5),

            /// <summary>
            /// Mouse cursor is currently over this node
            /// </summary>
            MouseOver = (1 << 6),

            /// <summary>
            /// Node should receive input events 
            /// </summary>
            Interactive = (1 << 7),

            /// <summary>
            /// True if the node is drawble
            /// </summary>
            Drawable = (1<<8)
        }

        private static List<Node> _pendingDestroy = new List<Node>();
        private static List<Node> _emptyList = new List<Node>();
        private Vector2 _scale = Vector2.One;
        private float _rotation = 0.0f;
        private List<Node> _children;
        private Node _parent;
        private Scene _scene;
        private Vector2 _position;
        private Rect _rect;
        private NodeVisibility _visibility;
        private Flags _flags;

        /// Transform used to convert local coordinates to scene coordinates
        private Matrix3 _localToScene;

        /// Transform used to convert scene coordinates to local coordinates
        private Matrix3 _sceneToLocal;

        /// <summary>
        /// The node's parent node
        /// </summary>
        public Node Parent => _parent;

        /// <summary>
        /// The scene that contains this node
        /// </summary>
        public Scene Scene => _scene;

        /// <summary>
        /// Current visibility of the node
        /// </summary>
        public NodeVisibility Visibility {
            get => _visibility;
            set {
                if (value == _visibility)
                    return;

                _visibility = value;
                UpdateVisible();
            }
        }

        /// <summary>
        /// Returns true if the node has all of the specified flags set
        /// </summary>
        private bool HasAllFlags(Flags flags) => (_flags & flags) == flags;

        /// <summary>
        /// Returs true if the node has any of the specified flags set
        /// </summary>
        private bool HasAnyFlags(Flags flags) => (_flags & flags) != 0;

        /// <summary>
        /// Set the given flags
        /// </summary>
        private void SetFlags(Flags flags) => _flags = _flags | flags;

        /// <summary>
        /// Unset the given flags
        /// </summary>
        /// <param name="flags"></param>
        private void ClearFlags(Flags flags) => _flags = _flags & (~flags);

        /// <summary>
        /// Set or unset given flags
        /// </summary>
        private void SetFlags(Flags flags, bool value) => _flags = value ? (_flags | flags) : (_flags & (~flags));


        public IEnumerable<Node> Children => _children ?? _emptyList;

        /// <summary>
        /// Returns true if the node is currently visibile within its scene.
        /// </summary>
        public bool IsVisible => HasAllFlags(Flags.Visible);

        /// <summary>
        /// Returns true if the mouse was over the node at the beginning of the frame
        /// </summary>
        public bool IsMouseOver => HasAllFlags(Flags.MouseOver);

        /// <summary>
        /// Returns true if the node has been destroyed
        /// </summary>
        public bool IsDestroyed => HasAllFlags(Flags.Destroyed);

        /// <summary>
        /// True if the node should have its Draw method called
        /// </summary>
        public bool IsDrawable {
            get => HasAllFlags(Flags.Drawable);
            set {
                if (value == IsDrawable)
                    return;

                SetFlags(Flags.Drawable, value);

                // If the node is already in a scene then invalidate the scenes draw list.
                Scene?.InvalidateDrawList();
            }
        }

        /// <summary>
        /// Return true if the node arranges its children
        /// </summary>
        public virtual bool DoesArrangeChildren => false;

        /// <summary>
        /// True if the transform of this node affects the transform of all of its children. When false
        /// the parent transform for all children will be identity.
        /// </summary>
        public virtual bool DoesTransformAffectChildren => true;

        /// <summary>
        /// True if the node arranges itself to its parent rect
        /// </summary>
        public virtual bool DoesArrangeToParent => false;

        /// <summary>
        /// True if the node should receive input events
        /// </summary>
        public bool IsInteractive {
            get => HasAllFlags(Flags.Interactive);
            set {
                SetFlags(Flags.Interactive, value);
            }
        }

        /// <summary>
        /// Position of the node within its parents coordinate space
        /// </summary>
        public Vector2 Position {
            get => _position;
            set {
                System.Diagnostics.Debug.Assert(!float.IsNaN(value.x));
                System.Diagnostics.Debug.Assert(!float.IsNaN(value.y));

                if (_position != value)
                {
                    _position = value;
                    InvalidateTransform();
                }
            }
        }

        /// <summary>
        /// Rectangle of the node within its parents coordinate space
        /// </summary>
        public Rect Rect {
            get {
                if (HasAllFlags(Flags.RectDirty))
                    UpdateRect();
                return _rect;
            }
        }

        /// <summary>
        /// Transforms local node coordinates to world coordinates
        /// </summary>
        public Matrix3 LocalToSceneMatrix {
            get {
                // Transforms are not updated until they are needed to update it now if its dirty
                if (HasAllFlags(Flags.TransformDirty))
                    UpdateTransform();
                return _localToScene;
            }
        }

        public Vector2 LocalToScene(in Vector2 local) => LocalToSceneMatrix.MultiplyVector(local);

        /// <summary>
        /// Transforms world coordinates to local node coordinates
        /// </summary>
        public Matrix3 SceneToLocalMatrix {
            get {
                // Transforms are not updated until they are needed to update it now if its dirty
                if (HasAllFlags(Flags.TransformDirty))
                    UpdateTransform();

                // WorldToLocal isnt calulcated unless its needed so calculate it now if its dirty
                if (HasAllFlags(Flags.WorldToLocalDirty))
                {
                    _sceneToLocal = _localToScene.Inverse();
                    ClearFlags(Flags.WorldToLocalDirty);
                }

                return _sceneToLocal;
            }
        }

        public Vector2 SceneToLocal(in Vector2 position) => SceneToLocalMatrix.MultiplyVector(position);

        public Matrix3 WindowToLocalMatrix => Matrix3.Multiply((Scene?.WindowToSceneMatrix) ?? Matrix3.Identity, SceneToLocalMatrix);

        public Vector2 WindowToLocal(in Vector2 position) => WindowToLocalMatrix.MultiplyVector(position);

        /// <summary>
        /// Opacity of the node and all of its children
        /// </summary>
        public float Opacity { get; set; } = 1.0f;

        /// <summary>
        /// Scale of the node and all of its children
        /// </summary>
        public Vector2 Scale {
            get => _scale;
            set {
                if (_scale != value)
                {
                    _scale = value;
                    InvalidateTransform();
                }
            }
        }
        public float ScaleX {
            get => _scale.x;
            set => Scale = new Vector2(value, _scale.y);
        }
        public float ScaleY {
            get => _scale.y;
            set => Scale = new Vector2(_scale.x, value);
        }

        /// <summary>
        /// Rotation of the node in degrees
        /// </summary>
        public float Rotation {
            get => _rotation;
            set {
                if (_rotation != value)
                {
                    _rotation = value;
                    InvalidateTransform();
                }
            }
        }

        public Node()
        {
            _visibility = NodeVisibility.Visible;
            _flags = Flags.Visible | Flags.AutoDestroy | Flags.RectDirty | Flags.TransformDirty;
        }

        /// <summary>
        /// Name of the node
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Returns the number of children the node has
        /// </summary>
        public int ChildCount => _children?.Count ?? 0;

        /// <summary>
        /// Return the child at the given index
        /// </summary>
        public Node GetChildAt(int index) => _children?[index] ?? null;

        /// <summary>
        /// Add a new child node
        /// </summary>
        public void AddChild(Node node) => InsertChild(_children?.Count ?? 0, node);

        /// <summary>
        /// Insert a child node at a given index
        /// </summary>
        public void InsertChild(int index, Node node)
        {
            if (node._parent != null)
            {
                node._parent._children.Remove(node);
                node._parent = null;
            }

            _children = _children ?? new List<Node>();
            _children.Insert(index, node);
            node._parent = this;
            node.InvalidateRect();
            node.InvalidateTransform();

            // If this is the scene then make sure the scene value gets 
            // set and propegated through all children.
            if (this is Scene && node._scene != this)
                node.PropegateScene(this as Scene);
            else if (Scene != node._scene)
                node.PropegateScene(Scene);

            node.OnParentChanged();

            void NotifyAnscestorChanged(Node notify)
            {
                notify.OnAnscestorChanged();
                if (notify._children != null)
                    foreach (var child in notify._children)
                        child.OnAnscestorChanged();
            }

            NotifyAnscestorChanged(node);
        }

        public void RemoveFromParent()
        {
            // Scene is top level and cannot be removed
            if (this is Scene)
                return;

            if (_parent != null)
                _parent._children.Remove(this);

            if (_scene != null)
                PropegateScene(null);

            _parent = null;
        }

        public void RemoveAllChildren()
        {
            if (null == _children)
                return;

            // Remove all children
            while (_children.Count > 0)
                _children[_children.Count - 1].RemoveFromParent();
        }

        private void PropegateScene(Scene scene)
        {
            if (_scene == scene)
                return;

            var old = _scene;
            if (old != null)
            {
                _scene.InvalidateDrawList();
                OnLeaveScene(old);
            }                

            _scene = scene;

            OnSceneChanged(old);

            if (_scene != null)
            {
                _scene.InvalidateDrawList();
                OnEnterScene(_scene);
            }                

            if (_children != null)
                foreach (var child in _children)
                    child.PropegateScene(scene);
        }

        /// <summary>
        /// Find first parent that matches the given type
        /// </summary>
        /// <typeparam name="T">Type to search for</typeparam>
        /// <returns>Node that matches the given type</returns>
        public T FindParent<T>() where T : Node
        {
            T result = null;
            for (var parent = Parent; result == null && parent != null; parent = parent.Parent)
                result = parent as T;

            return result;
        }

        /// <summary>
        /// Find a child node with the given name a depth first search
        /// </summary>
        /// <param name="name">Name to seach for</param>
        /// <returns>First matching node</returns>
        public Node FindChild(string name)
        {
            if (_children == null)
                return null;

            Node result = null;
            for (int i = 0, c = _children.Count; i < c && null == result; i++)
                result = _children[i].Name == name ? _children[i] : _children[i].FindChild(name);

            return result;
        }

        /// <summary>
        /// Find first child that is castable to the given type using a depth first search
        /// </summary>
        /// <typeparam name="T">Type to search for</typeparam>
        /// <returns>Node that matches the given type</returns>
        public T FindChild<T>() where T : Node
        {
            if (_children == null)
                return null;

            T result = null;
            for (int i = 0, c = _children.Count; i < c && null == result; i++)
                result = (_children[i] as T) ?? _children[i].FindChild<T>();

            return result;
        }

        /// <summary>
        /// Internal method used by FindChildren to recurse
        /// </summary>
        /// <typeparam name="T">Node Type to search for</typeparam>
        /// <param name="list">Accumulated list of nodes matching given type</param>
        private void FindChildrenInternal<T>(List<T> list) where T : Node
        {
            if (_children == null)
                return;

            for (int i = 0, c = _children.Count; i < c; i++)
            {
                if (typeof(T).IsAssignableFrom(_children[i].GetType()))
                    list.Add(_children[i] as T);

                _children[i].FindChildrenInternal(list);
            }
        }

        /// <summary>
        /// Find first child that is castable to the given type using a depth first search
        /// </summary>
        /// <typeparam name="T">Type to search for</typeparam>
        /// <returns>Node that matches the given type</returns>
        public Node[] FindChildren<T>() where T : Node
        {
            var results = new List<T>();
            FindChildrenInternal(results);
            return results.ToArray();
        }

        /// <summary>
        /// Invalidate the transform of the node and the transform of all descendants as well
        /// </summary>
        public void InvalidateTransform()
        {
            if (HasAllFlags(Flags.TransformDirty))
                return;

            SetFlags(Flags.TransformDirty);

            // Invalidate transform of all children as well
            if (_children != null && DoesTransformAffectChildren)
                foreach (var child in _children)
                    child.InvalidateTransform();
        }

        /// <summary>
        /// Update the nodes transform
        /// </summary>
        public void UpdateTransform()
        {
            if (!HasAllFlags(Flags.TransformDirty))
                return;
            
            // Make sure the rectangle is updated too
            UpdateRect();

            // Find our deepest ancestor that has a dirty transform and update them instead.
            if (_parent != null && _parent.HasAllFlags(Flags.TransformDirty) && _parent.DoesTransformAffectChildren)
                _parent.UpdateTransform();

            ClearFlags(Flags.TransformDirty);

            var mat = Matrix3.Identity;
            mat = Matrix3.Multiply(mat, Matrix3.Scale(Scale));
            mat = Matrix3.Multiply(mat, Matrix3.Rotate(Rotation * MathEx.Deg2Rad));
            mat = Matrix3.Multiply(mat, Matrix3.Translate(Position));

            // Apply the parent transform..
            if (_parent == null || !_parent.DoesTransformAffectChildren)
                _localToScene = mat;
            else
                _localToScene = Matrix3.Multiply(mat, _parent._localToScene);

            SetFlags(Flags.WorldToLocalDirty);
        }

        /// <summary>
        /// Invalidate the frame an all child frames that are dependant
        /// </summary>
        public void InvalidateRect()
        {
            // Invalidate our parent as well if our parent arranges us
            if (_parent != null && _parent.DoesArrangeChildren)
                _parent.InvalidateRect();

            if (HasAllFlags(Flags.RectDirty))
                return;

            SetFlags(Flags.RectDirty);

            // If parent arranges children then also invalidate the parent
            if (_parent != null && _parent.DoesArrangeChildren)
                _parent.InvalidateRect();

            // Invalidate all children if this node arranges children
            if (_children != null)
            {
                if (DoesArrangeChildren)
                    foreach (var child in _children)
                        child.InvalidateRect();
                else
                    foreach (var child in _children)
                        if (child.DoesArrangeToParent)
                            child.InvalidateRect();
            }
        }

        /// <summary>
        /// Called by parent node to instruct the node to arrange itself using 
        /// the given rectangle.
        /// </summary>
        private void Arrange(in Rect rect)
        {
            // Arrange the node using the given rect
            var newRect = ArrangeOverride(rect);

            // Calculate the position using the nodes pivot and the resulting rect
            var position = newRect.TopLeft + newRect.Size * GetPivot();

            Position = position;

            SetRect(newRect.Offset(-position));
        }

        /// <summary>
        /// Arranges a child node using the given rectangle
        /// </summary>
        protected void ArrangeChild (int index, in Rect rect) => GetChildAt(index).Arrange(rect);

        /// <summary>
        /// Override to implement custom arrange logic
        /// </summary>
        protected virtual Rect ArrangeOverride(in Rect rect) => rect;

        /// <summary>
        /// Override to supply a custom pivot 
        /// </summary>
        protected virtual Vector2 GetPivot() => Vector2.Half;

        /// <summary>
        /// Measure the size of the node
        /// </summary>
        public Vector2 Measure(in Vector2 available) => MeasureOverride(available);

        /// <summary>
        /// Override to customize measurement for a node
        /// </summary>
        protected virtual Vector2 MeasureOverride(in Vector2 available) => Vector2.Zero;

        /// <summary>
        /// Override to arrange children within the node
        /// </summary>
        protected virtual void ArrangeChildren(in Rect rect) { }

        /// <summary>
        /// Updates the Rect value for the node
        /// </summary>
        public void UpdateRect()
        {
            // Dont update if rect isnt invalid
            if (!HasAllFlags(Flags.RectDirty))
                return;

            // If we arrange to our parent rectangle or our parent arranges us then
            // make sure the parent rectangle is updated before we handle our own rectangle.
            if(Parent != null && (DoesArrangeToParent || Parent.DoesArrangeChildren))
                Parent.UpdateRect();

            // Check one more time if the rect is dirty since updating the parent rectangle may
            // have inadvertently updated us.
            if (!HasAllFlags(Flags.RectDirty))
                return;

            // If this node arranges to its parent and its parent isnt arranging children 
            // then it can be arranged now using its parent rectangle.
            if(DoesArrangeToParent && Parent != null && !Parent.DoesArrangeChildren)
            {
                Arrange(Parent.Rect);
                return;
            }

            // Default handling is to just measure the node and use its measurement to 
            // determine the rectangle
            var measured = Measure(Vector2.Zero);
            SetRect(new Rect(-measured * GetPivot(), measured));
        }

        /// <summary>
        /// Set the node rectangle
        /// </summary>
        private void SetRect (in Rect rect)
        {
            // Clear the dirty rect flag
            ClearFlags(Flags.RectDirty);

            if (_rect != rect)
            {
                _rect = rect;
                OnRectChanged(_rect);

                // Let all of our children know our frame changed too
                if (_children != null)
                    foreach (var child in _children)
                        child.OnParentRectChanged(Rect);
            }

            if (DoesArrangeChildren)
                ArrangeChildren(_rect);
        }

        /// <summary>
        /// Internal method used to update the Visibile flag within the node and and child nodes.
        /// </summary>
        private void UpdateVisible()
        {
            // Calculate new visible state.
            var visible = (Parent == null || Parent.IsVisible) && Visibility == NodeVisibility.Visible;
            if (IsVisible == visible)
                return;

            // Set new visible state
            SetFlags(Flags.Visible, visible);

            // Propegate to all children
            if (_children != null)
                for (var i = _children.Count - 1; i >= 0; i--)
                    _children[i].UpdateVisible();

            // Give the node a chance to handle its own visibility state.
            OnVisibleChanged(visible);

            // When the visibility changes a parent that arranges children will need 
            // to adjust its arrangement due to the visibility change.
            if (Parent != null && Parent.DoesArrangeChildren)
                Parent.InvalidateRect();
        }

        /// <summary>
        /// Override to draw a node using the given graphics context.  This method will be called 
        /// on all nodes with IsDrawable equal to true
        /// </summary>
        public virtual void Draw(GraphicsContext gc) { }

        public virtual void DrawEnd (GraphicsContext gc) { }

        protected virtual void OnRectChanged(in Rect rect) { }

        protected virtual void OnParentRectChanged(in Rect rect) { }

        protected virtual void OnVisibleChanged(bool visible) { }

        protected virtual void OnParentChanged() { }

        protected virtual void OnAnscestorChanged() { }

        protected virtual void OnSceneChanged(Scene oldScene) { }

        protected virtual void OnEnterScene(Scene entering) { }

        protected virtual void OnLeaveScene(Scene leaving) { }

        protected virtual void OnMouseEnter() { }

        protected virtual void OnMouseLeave() { }

        protected internal virtual void OnMouseOver(MouseOverEvent e) { }

        protected internal virtual void OnMouseDown(MouseButtonEvent e) { }

        protected internal virtual void OnMouseUp(MouseButtonEvent e) { }

        protected virtual void OnDestroy()
        {
            DestroyEvent.Broadcast(this);
        }

        /// <summary>
        /// Destroy a node and all of its children them to be removed from the scene and disposed of.
        /// </summary>
        public void Destroy()
        {
            if (IsDestroyed)
                return;

            UnsubscribeAll();

            // Stop all state machines running on this node.
            StateMachine.Stop(this);

            // Flag the node as destroyed and add to the destroy list
            SetFlags(Flags.Destroyed);
            _pendingDestroy.Add(this);

            // Destroy all children as well
            if (_children != null)
                for (int i = 0; i < _children.Count; i++)
                    _children[i].Destroy();
        }

        public static void ProcessDestroyedNodes()
        {
            for (int i = 0; i < _pendingDestroy.Count; i++)
            {
                var node = _pendingDestroy[i];

                // Remove ourself from the tree
                node.RemoveFromParent();

                // Destroy ourself
                node.OnDestroy();

                // Automatically remove any observers of the node since we know it is no longer
                // going to be sending events.
                Event.UnsubscribeAllObservers(node);
            }
        }

        /// <summary>
        /// Determines if the given worldPosition is is within the boundaries of the 
        /// nodes rectangle.
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns>True if the position is within the node's bounds.</returns>
        public virtual HitTestResult HitTest(in Vector2 worldPosition)
        {
            if (!IsVisible)
                return HitTestResult.Ignore;

            if (IsInteractive)
            {
                var position = SceneToLocal(worldPosition);
                if (Rect.Contains(position))
                    return HitTestResult.Hit;
            }

            return HitTestResult.NotHit;
        }

        /// <summary>
        /// Update all mouse over flags for the node and all of its descendants
        /// and return the top most node that the mouse is over.
        /// </summary>
        /// <returns></returns>
        internal static Node UpdateMouseOvers()
        {
            Node oldMouseOver = Input.MouseOver;
            Node newMouseOver = null;
            for (int i = Window.ViewCount - 1; i >= 0 && null == newMouseOver; i--)
            {
                var view = Window.GetViewAt(i);
                if (view.Scene == null)
                    continue;

                if (view.Scene.IsPaused)
                    continue;

                newMouseOver = view.Scene.GetNodeAtPoint(null, view.Scene.WindowToScene(Input.MousePosition));

                // If a scene is marked interactive then it blocks all input below it
                if (newMouseOver == null && view.Scene.IsInteractive)
                    newMouseOver = view.Scene;
            }

            // No change?
            if (oldMouseOver == newMouseOver)
                return newMouseOver;

            // Turn on the mouse over flag for the new mouse over node and any parents until
            // we run out of nodes or hit a node that already has the parent set.
            var node = newMouseOver;
            while (node != null && !node.IsMouseOver)
            {
                node.SetFlags(Flags.MouseOver);
                node.OnMouseEnter();
                node = node.Parent;
            }

            // Walk up the parent list of the old mouse over and clear the flag until we hit
            // either the node common between the old and new or run out of nodes.
            if (oldMouseOver != null && node != oldMouseOver)
            {
                for (; oldMouseOver != null && oldMouseOver != node; oldMouseOver = oldMouseOver.Parent)
                {
                    oldMouseOver.ClearFlags(Flags.MouseOver);
                    oldMouseOver.OnMouseLeave();
                }
            }

            return newMouseOver;
        }

        /// <summary>
        /// Return the accumulated opacity from all parents
        /// </summary>
        public float AccumulatedOpacity {
            get {
                var opacity = Opacity;
                for (var parent = Parent; parent != null && opacity > 0.0f; parent = parent.Parent)
                    opacity *= parent.Opacity;
                return opacity;
            }
        }
    }
}

