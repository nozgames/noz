
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

        /// <summary>
        /// Name of the node
        /// </summary>
        public string Name { get; set; }

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

        /// <summary>
        /// Find a child node with the given name a depth first search
        /// </summary>
        /// <param name="name">Name to seach for</param>
        /// <returns>First matching node</returns>
        public Node FindChild (string name)
        {
            Node result = null;
            for (int i = 0, c = _children.Count; i < c && null == result; i++)
                result = _children[i].Name == name ? _children[i] : _children[i].FindChild(name);

            return result;
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

