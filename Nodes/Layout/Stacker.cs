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
    public class Stacker : Layout
    {
        private Vector2[] _measures;

        /// <summary>
        /// Amount of spacing between child nodes
        /// </summary>
        public float Spacing { get; set; } = 0.0f;

        /// <summary>
        /// Direction child nodes are stacked
        /// </summary>
        public Orientation Orientation { get; set; } = Orientation.Vertical;

        /// <summary>
        /// When the stackers frame changes arrange all children in a stack
        /// </summary>
        /// <param name="frame">new frame value</param>
        protected override void OnRectChanged(in Rect rect)
        {
            // The U axis is the axis which content grows
            var u_axis = Orientation == Orientation.Vertical ? 1 : 0;

            // Arrange all children that are not collapsed
            var childFrame = Rect;
            childFrame[u_axis + 2] = 0;

            for(int i=0, c=ChildCount; i<c; i++)
            {
                var child = GetChildAt(i);
                if (!child.IsVisible)
                    continue;

                childFrame[u_axis + 2] = _measures[i][u_axis];
                child.Arrange(childFrame);
                childFrame[u_axis] += (childFrame[u_axis + 2] + Spacing);
            }
        }

        /// <summary>
        /// Measure all children in a stack formation
        /// </summary>
        /// <param name="available"></param>
        /// <returns></returns>
        protected override Vector2 MeasureChildren (in Vector2 available)
        {
            if(ChildCount == 0)
            {
                _measures = null;
                return Vector2.Zero;
            }

            var size = Vector2.Zero;

            // The U axis is the axis which content grows
            var u_axis = Orientation == Orientation.Vertical ? 1 : 0;

            // The V axis is the axis which will have a fixed size
            var v_axis = u_axis ^ 1;

            var count = 0;

            if (_measures == null || _measures.Length != ChildCount)
                _measures = new Vector2[ChildCount];            

            for(int i=0,c=ChildCount; i<c; i++)
            {
                var child = GetChildAt(i);
                if(!child.IsVisible)
                    continue;

                _measures[i] = child.Measure(available);
                size[v_axis] = Math.Max(size[v_axis], _measures[i][v_axis]);
                size[u_axis] += _measures[i][u_axis];
                count++;
            }

            // Add in the spacing to the measure
            if (count > 1)
                size[u_axis] += (Spacing * (count - 1));

            return size;
        }
    }
}
