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
    public enum HorizontalAlignment
    {
        Left,
        Right,
        Center,
        Stretch
    }

    public enum VerticalAlignment
    {
        Top,
        Bottom,
        Center,
        Stretch
    }

    public class Layout : Node
    {
        public override bool DoesArrangeChildren => true;
        public override bool DoesArrangeToParent => true;

        private Vector2 _size;
        private Vector2 _minSize;
        private Vector2 _maxSize;

        public float Width { get => _size.x; set => _size.x = value; }
        public float Height { get => _size.y; set => _size.y = value; }
        public float MinWidth { get => _minSize.x; set => _minSize.x = value; }
        public float MinHeight { get => _minSize.y; set => _minSize.y = value; }
        public float MaxWidth { get => _maxSize.x; set => _maxSize.x = value; }
        public float MaxHeight { get => _maxSize.y; set => _maxSize.y = value; }
        public Vector2 Size { get => _size; set => _size = value; }
        public Vector2 MinSize { get => _minSize; set => _minSize = value; }
        public Vector2 MaxSize { get => _maxSize; set => _maxSize = value; }
        public Thickness Margin { get; set; }
        public Thickness Padding { get; set; }
        public Vector2 Pivot { get; set; }

        public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Stretch;

        public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Stretch;

        public Layout()
        {
            _size = Vector2.NaN;
            _maxSize = Vector2.Infinity;
            _minSize = Vector2.Zero;
            //AspectRatio = Vector2.NaN;
            Padding = Thickness.Empty;
            Margin = Thickness.Empty;
            Pivot = Vector2.Half;
        }

        /// <summary>
        /// Returns true if the given value is set to "Automatic"
        /// </summary>
        /// <param name="value">Float value to check.</param>
        /// <returns>True if the given float is "Automatic"</returns>
        private static bool IsAuto(float value) => float.IsNaN(value);

        /// <summary>
        /// Route the layout's pivot value through to the base node
        /// </summary>
        protected override Vector2 GetPivot() => Pivot;

        /// <summary>
        /// Arrange ourselv to the given rectangle
        /// </summary>
        /// <param name="rect"></param>
        protected override Rect ArrangeOverride (in Rect rect) {
            // Measure ourself
            var measuredSize = Measure(rect.Size);

            // Calculate the actual size of the node
            var actualMargin = Margin;

            switch (HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    actualMargin.right += (rect.width - measuredSize.x);
                    break;

                case HorizontalAlignment.Right:
                    actualMargin.left += (rect.width - measuredSize.x);
                    break;

                case HorizontalAlignment.Stretch:
                    if (!IsAuto(Width))
                        goto case HorizontalAlignment.Center;

                    break;

                case HorizontalAlignment.Center:
                {
                    var extraMargin = (rect.width - measuredSize.x) * 0.5f;
                    actualMargin.left += extraMargin;
                    actualMargin.right += extraMargin;
                    break;
                }
            }

            switch (VerticalAlignment)
            {
                case VerticalAlignment.Top:
                    actualMargin.bottom += (rect.height - measuredSize.y);
                    break;

                case VerticalAlignment.Bottom:
                    actualMargin.top += (rect.height - measuredSize.y);
                    break;

                case VerticalAlignment.Stretch:
                    if (!IsAuto(Height))
                        goto case VerticalAlignment.Center;
                    break;

                case VerticalAlignment.Center:
                {
                    var extraMargin = (rect.height - measuredSize.y) * 0.5f;
                    actualMargin.top += extraMargin;
                    actualMargin.bottom += extraMargin;
                    break;
                }
            }

            return new Rect(
                rect.x + actualMargin.left,
                rect.y + actualMargin.top,
                rect.width - actualMargin.left - actualMargin.right,
                rect.height - actualMargin.bottom - actualMargin.top
                );
        }

        protected sealed override Vector2 MeasureOverride (in Vector2 available)
        {
            var adjustedAvailable = available;
            var autoX = IsAuto(_size.x);
            var autoY = IsAuto(_size.y);

            // If auto-sizing then reduce the available by our margin since our 
            // children will have less space because of our margins.  If not auto
            // sizing then the size available is our size.
            if (autoX)
                adjustedAvailable.x -= (Margin.left + Margin.right);
            else
                adjustedAvailable.x = _size.x;

            if (autoY)
                adjustedAvailable.y -= (Margin.top + Margin.bottom);
            else
                adjustedAvailable.y = _size.y;

            // Reduce available size by the padding
            adjustedAvailable.x -= Padding.left;
            adjustedAvailable.x -= Padding.right;
            adjustedAvailable.y -= Padding.top;
            adjustedAvailable.y -= Padding.bottom;

            // Measure all children
            var measuredSize = Vector2.Max(Vector2.Zero, MeasureChildren(adjustedAvailable));

            // Force our measured size to by our fixed size
            if (!autoX) measuredSize.x = _size.x;
            if (!autoY) measuredSize.y = _size.y;

            // Add padding
            if (autoX) measuredSize.x += (Padding.left + Padding.right);
            if (autoY) measuredSize.y += (Padding.top + Padding.bottom);

            // Minimum/Maximum size.
            measuredSize = Vector2.Clamp(measuredSize, _minSize, _maxSize);

            measuredSize.x += (Margin.left + Margin.right);
            measuredSize.y += (Margin.bottom + Margin.top);

            return measuredSize;
        }

        /// <summary>
        /// Measure all child nodes 
        /// </summary>
        protected virtual Vector2 MeasureChildren (in Vector2 available)
        {
            var size = Vector2.Zero;
            foreach (var child in Children)
                size = Vector2.Max(size, child.Measure(available));

            return size;
        }

        /// <summary>
        /// Arrange all child nodes
        /// </summary>
        protected sealed override void ArrangeChildren (in Rect rect) => 
            ArrangeChildrenOverride(rect.Contract(Padding));

        /// <summary>
        /// Override to arrange all child nodes.  Default implementation arranges all children
        /// to match the layout rectangle minus padding.
        /// </summary>
        protected virtual void ArrangeChildrenOverride (in Rect rect)
        {
            for (int i = 0; i < ChildCount; i++)
                ArrangeChild(i, rect);
        }
    }
}
