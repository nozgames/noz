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
    public enum DrawNodeType : byte
    {
        Quad,
        Group,
        DebugLine
    }

    public class DrawNode
    {
        /// <summary>
        /// Type of node
        /// </summary>
        public DrawNodeType Type;

        /// <summary>
        /// Quad used to render the draw node
        /// </summary>
        public Quad quad;

        /// <summary>
        /// Image used to render the draw node
        /// </summary>
        public Image image;

        /// <summary>
        /// Color to render the node
        /// </summary>
        public Color color;

        /// <summary>
        /// Index of the node
        /// </summary>
        public ushort Index;

        /// <summary>
        /// Sort group
        /// </summary>
        public ushort SortGroup;

        /// <summary>
        /// Sort layer within the sort group
        /// </summary>
        public byte SortLayer;

        /// <summary>
        /// Sort order within the sort layer
        /// </summary>
        public short SortOrder;

        /// <summary>
        /// Value used to sort nodes for transparency
        /// </summary>
        public float TransparencySort;
        
        /// <summary>
        /// Start index of nodes for group
        /// </summary>
        public ushort GroupStart;

        /// <summary>
        /// Number of nodes in group
        /// </summary>
        public ushort GroupCount;

        public class Comparer : IComparer<DrawNode>
        {
            public static Comparer Instance = new Comparer();

            public int Compare(DrawNode lhs, DrawNode rhs)
            {
                var diff1 = lhs.SortGroup - rhs.SortGroup;
                if (diff1 != 0) return diff1;

                var diff2 = lhs.SortLayer - rhs.SortLayer;
                if (diff2 != 0) return diff2;

                var diff3 = lhs.SortOrder - rhs.SortOrder;
                if (diff3 != 0) return lhs.SortOrder < rhs.SortOrder ? -1 : 1;

                var tdiff = lhs.TransparencySort - rhs.TransparencySort;
                if (tdiff != 0.0f) return (int)MathEx.Sign(tdiff);

                return lhs.Index - rhs.Index;
            }
        }
    }
}
