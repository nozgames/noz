/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

namespace NoZ
{
    public class Node : IDisposable
    {
        private List<Node>? _children;
        private Node? _parent;
        private int _siblingIndex = -1;
        internal Scene? _scene;

        /// <summary>
        /// True if the node is part of the world or false if an orphaned node
        /// </summary>
        public bool IsInWorld => _scene != null;

        /// <summary>
        /// Return the number of children the parent has
        /// </summary>
        public int ChildCount => _children?.Count ?? 0;

        /// <summary>
        /// Return the index of the node within the parent
        /// </summary>
        public int SiblingIndex => _siblingIndex;

        /// <summary>
        /// Return the parent node.
        /// </summary>
        public Node? Parent => _parent;

        /// <summary>
        /// Scene the node belongs to
        /// </summary>
        public Scene? Scene => _scene;

        virtual public void Dispose()
        {
            if (_children != null)
            {
                for (int i = _children.Count - 1; i >= 0; i--)
                    _children[i].Dispose();

                _children.Clear();
            }

            Orphan();
        }

        /// <summary>
        /// Parent a node to another node
        /// </summary>
        public void SetParent(Node parent) =>
            SetParentInternal(parent);

        public void SetParent(Scene scene) =>
            SetParentInternal(scene?.RootNode);

        public void Orphan() =>
            SetParentInternal(null);

        private void SetParentInternal(Node? parent)
        {
            if (_parent != null)
            {
                _parent._children!.RemoveAt(_siblingIndex);
                for (int i = _siblingIndex; i < _parent._children.Count; i++)
                    _parent._children[i]._siblingIndex = i;

                _siblingIndex = -1;

                _parent.OnRemoveChild(this);

                if (_scene != null)
                {                    
                    OnExitWorld();
                    _scene = null;
                }
            }

            _parent = parent;

            if (_parent != null)
            {
                _parent._children ??= new List<Node>();
                _parent._children.Add(this);

                _scene = _parent._scene;
                _siblingIndex = _parent._children.Count - 1;

                _parent.OnAddChild(this);

                if (_scene != null)
                    OnEnterWorld();
            }
        }

        public void Add(Node node) =>
            node.SetParent(this);

        /// <summary>
        /// Return the child node at the given index
        /// </summary>
        public Node GetChild(int index) => _children![index];


        protected virtual void OnEnterWorld()
        {
            _scene?.RegisterNode(this);

            // Inform all of the children that they are now in the world
            if (_children != null)
            {
                for (int i = 0; i < _children.Count; i++)
                {
                    _children[i]._scene = _scene;
                    _children[i].OnEnterWorld();
                }                    
            }
        }

        protected virtual void OnExitWorld()
        {
            // Inform all of the children that they are now in the world
            if (_children != null)
            {
                for (int i = 0; i < _children.Count; i++)
                {
                    _children[i]._scene = null;
                    _children[i].OnExitWorld();
                }
            }

            _scene!.UnregisterNode(this);
        }

        protected virtual void OnAddChild(Node child)
        {
        }

        protected virtual void OnRemoveChild(Node child)
        {
        }

        /// <summary>
        /// Returns the parent node as the given node type or throws an exception if the parent is not of the given type
        /// </summary>
        public TNode GetParent<TNode>() where TNode : Node =>
            (TNode)_parent!;
    }
}
