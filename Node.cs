using System;
using System.Collections.Generic;

namespace NoZ
{
    public class Node
    {
        private static List<Node> _emptyList = new List<Node>();
        private Vector2 _scale = Vector2.One;
        private List<Node> _children;
        private Node _parent;
        private Scene _scene;
        private Vector2 _position;
        private bool _frameIsDirty = true;
        private Rect _frame;

        /// <summary>
        /// The node's parent node
        /// </summary>
        public Node Parent => _parent;

        /// <summary>
        /// The scene that contains this node
        /// </summary>
        public Scene Scene => _scene;

        public IEnumerable<Node> Children => _children ?? _emptyList;

        public bool IsVisible { get; set; } = true;

        public virtual bool DoesArrangeChildren => false;

        public Vector2 Position {
            get => _position;
            set {
                _position = value;
                _frameIsDirty = true;
            }
        }

        public Rect Frame {
            get {
                if (_frameIsDirty)
                    UpdateFrame();
                return _frame;
            }
        }

        public Rect AccumulatedFrame {
            get {
                return new Rect();
            }
        }

        public float Alpha {
            get; set;
        } = 1.0f;

        public Vector2 Scale {
            get => _scale;
            set => _scale = value;
        }
        public float ScaleX {
            get => _scale.x;
            set => _scale.x = value;
        }
        public float ScaleY {
            get => _scale.y;
            set => _scale.y = value;
        }

        public float Rotation {
            get; set;
        }

        public int ChildCount => _children?.Count ?? 0;

        public Node GetChildAt(int index) => _children?[index] ?? null;

        /// <summary>
        /// Add a new child node
        /// </summary>
        /// <param name="node"></param>
        public void AddChild(Node node) => InsertChild(_children?.Count ?? 0, node);

        public void InsertChild(int index, Node node)
        {
            if (node._parent != null)
            {
                node._parent._children.Remove(node);
                node._parent = null;
            }

            // If this is the scene then make sure the scene value gets 
            // set and propegated through all children.
            if (this is Scene && node._scene != this)
                node.PropegateScene(this as Scene);

            _children = _children ?? new List<Node>();
            _children.Insert(index, node);
            node._parent = this;
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
            _scene = scene;
            foreach (var child in Children)
                child.PropegateScene(scene);
        }

        public virtual Vector2 Measure(in Vector2 available) => Vector2.Zero;

        /// <summary>
        /// Called by parent node to instruct the node to arrange itself using 
        /// the given rectangle.
        /// </summary>
        /// <param name="rect"></param>
        public virtual void Arrange(Rect rect) { }

        /// <summary>
        /// Invalidate the frame an all child frames that are dependant
        /// </summary>
        public void InvalidateFrame()
        {
            _frameIsDirty = true;

            if(_children != null && DoesArrangeChildren)
                foreach (var child in _children)
                    child.InvalidateFrame();
        }

        private void UpdateFrame()
        {
            // Ensure all parents are updated as well
            if (_parent != null)
                _parent.UpdateFrame();

            if (!_frameIsDirty)
                return;

            _frameIsDirty = false;

            var oldFrame = _frame;
            _frame = CalculateFrame();
            if (_frame == oldFrame)
                return;

            OnFrameChanged(oldFrame);

            // Let all of our children know our frame changed too
            if (_children != null)
            {
                foreach (var child in _children)
                {
                    child.OnParentFrameChanged(Frame);
                }
            }
        }

        protected virtual void OnFrameChanged(in Rect frame) { }

        protected virtual void OnParentFrameChanged(Rect frame) { }

        protected virtual Rect CalculateFrame() => new Rect(Position, Vector2.Zero);
    }
}

