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

using System.Collections.Generic;

namespace NoZ.Graphics
{
    /// <summary>
    /// List of all nodes that will be drawn sorted by their layer and sort order.  The draw list has a 
    /// maximum of 65535 drawable nodes.
    /// </summary>
    class DrawList
    {
        public struct DrawNode
        {
            public ushort PaintersIndex;
            public ushort SortGroup;
            public int SortOrder;
            public Node Node;

            public class Comparer : IComparer<DrawNode> {
                public static Comparer Instance = new Comparer();

                public int Compare(DrawNode x, DrawNode y) {
                    var diff = x.SortOrder - y.SortOrder;
                    if (diff != 0) return diff;

                    return x.PaintersIndex - y.PaintersIndex;
                }
            }
        }

        public struct DrawLayer
        {
            /// <summary>
            /// Node that the layer represents
            /// </summary>
            public Node Node;

            /// <summary>
            /// Start index of the layer within the _nodes list.
            /// </summary>
            public ushort Start;

            /// <summary>
            /// Number of DrawNode's in the layer.
            /// </summary>
            public ushort Count;
        }

        private List<DrawNode> _nodes = new List<DrawNode>(4096);
        private List<DrawLayer> _layers = new List<DrawLayer>(64);

        private void BuildLayer(ushort layerIndex)
        {
            var drawLayer = _layers[layerIndex];
            drawLayer.Start = (ushort)_nodes.Count;

            var node = drawLayer.Node;

            // Add all children recursively
            for (int i = 0, c = drawLayer.Node.ChildCount; i<c; i++)
                AddNode(drawLayer.Node.GetChildAt(i));

            // Update the layer count to reflect all of the nodes within the layer
            drawLayer.Count = (ushort)(_nodes.Count - drawLayer.Start);

            // Sort all nodes in the layer against each other
            _nodes.Sort(drawLayer.Start, drawLayer.Count, DrawNode.Comparer.Instance);

            // Push the updated draw layer back into the layers list
            _layers[layerIndex] = drawLayer;
        }

        private void AddNode(Node node) {
            // Skip nodes and their descendants if they are not visible.
            if (!node.IsVisible) return;

            var layer = node as ILayer;
            if (null != layer) {
                var drawLayer = new DrawLayer();
                drawLayer.Count = 0;
                drawLayer.Start = 0;
                drawLayer.Node = node;
                _layers.Add(drawLayer);

                var sd = new DrawNode();
                sd.Node = node;
                sd.PaintersIndex = (ushort)_nodes.Count;
                sd.SortOrder = layer.SortOrder;
                sd.SortGroup = (ushort)(_layers.Count - 1);
                _nodes.Add(sd);
                return;
            }

            var drawable = node as IDrawable;
            if (drawable != null) {
                var sd = new DrawNode();
                sd.Node = node;
                sd.PaintersIndex = (ushort)_nodes.Count;
                sd.SortOrder = drawable.SortOrder;
                sd.SortGroup = 0;
                _nodes.Add(sd);
            }

            for(int i=0, c=node.ChildCount; i<c; i++) {
                AddNode(node.GetChildAt(i));
            }
        }

        public void Build(Node node) {
            _nodes.Clear();
            _layers.Clear();

            var rootGroup = new DrawLayer();
            rootGroup.Node = node;
            rootGroup.Start = 0;
            rootGroup.Count = 0;
            _layers.Add(rootGroup);

            // Build all layers.  Note that building a layer may add
            // more layers which is handled by the loop.
            for (int i = 0; i < _layers.Count; i++)
                BuildLayer((ushort)i);
        }

        private void Draw (GraphicsContext gc, int layerIndex) {
            var drawLayer = _layers[layerIndex];

            var layer = drawLayer.Node as ILayer;
            layer.BeginLayer(gc);

            var layerDrawable = drawLayer.Node as IDrawable;
            layerDrawable?.Draw(gc);

            for (int i = drawLayer.Start, e = drawLayer.Start + drawLayer.Count; i < e; i++) {
                if (_nodes[i].SortGroup != 0) {
                    Draw(gc, _nodes[i].SortGroup);
                    continue;
                }

                var drawable = _nodes[i].Node as IDrawable;
                gc.SetTransform(_nodes[i].Node.LocalToWorld);
                drawable?.Draw(gc);
            }

            layer.EndLayer(gc);
        }

        public void Draw(GraphicsContext gc) {
            Draw(gc, 0);
        }

        public void Clear ()
        {
            _nodes.Clear();
            _layers.Clear();
        }
    }
}
