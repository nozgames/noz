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

using NoZ.Graphics;

namespace NoZ
{
    public class Sprite : Node
    {
        private Vector2 _size;
        private Vector2 _pivot = Vector2.Half;

        public Image Image { get; set; }

        public Vector2 Size {
            get => _size;
            set {
                _size = value;
                InvalidateFrame();
            }
        }
        public Vector2 Pivot {
            get => _pivot;
            set {
                _pivot = value;
                InvalidateFrame();
            }
        }

        public Sprite (Image image)
        {
            Image = image;
        }

        /// <summary>
        /// If arranged adjust the sprites size to match the given rectangle
        /// </summary>
        /// <param name="frame"></param>
        public override void Arrange(Rect frame)
        {
            // Update size to match the arranged rectangle
            Size = frame.Size;

            // Update position to reflect the new size
            Position = new Vector2(
                frame.x + frame.width * Pivot.x,
                frame.y + frame.height * Pivot.y);
        }

        public override Vector2 Measure(in Vector2 available) => Size;

        protected override Rect CalculateFrame() => new Rect(Position - _size * Pivot, _size);
    }
}
