/*
  NozEngine Library

  Copyright(c) 2015 NoZ Games, LLC

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
    public enum SpriteDrawMode
    {
        Auto,
        Stretched,
        Sliced,
        SlicedNoFill
    }

    public class Sprite : Node
    {
        private Quad[] _quads;
        private Image _image;
        private Vector2 _pivot = Vector2.Half;

        /// <summary>
        /// True if the vertex buffer and index buffer need to be updated.
        /// </summary>
        private bool _meshInvalid = true;

        /// <summary>
        /// Color of sprite
        /// </summary>
        public Color Color { get; set; } = Color.White;

        /// <summary>
        /// Sprite draw mode
        /// </summary>
        public SpriteDrawMode DrawMode { get; set; } = SpriteDrawMode.Auto;

        /// <summary>
        /// Sprite sort layer
        /// </summary>
        public int SortLayer { get; set; }

        /// <summary>
        /// Sprite sort order
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Sprite pivot
        /// </summary>
        public Vector2 Pivot {
            get => _pivot;
            set {
                _pivot = value;
                InvalidateRect();
            }
        }

        /// <summary>
        /// Image used to render the rectangle.  If no image is given a solid color rectangle 
        /// will be rendered instead by using a solid white texture.
        /// </summary>
        public virtual Image Image {
            get => _image;
            set {
                if (_image == value)
                    return;

                _image = value;
                IsDrawable = _image != null;
                InvalidateRect();
            }
        }

        public MaskMode MaskMode { get; set; } = MaskMode.Inside;

        public Sprite ( )
        {
        }

        public Sprite (Image image)
        {
            Image = image;
        }

        public Sprite (Image image, Color color)
        {
            Image = image;
            Color = color;
        }

        public Sprite (Image image, Color color, SpriteDrawMode drawMode)
        {
            Image = image;
            Color = color;
            DrawMode = drawMode;
        }

        private Vector2 MeasureSliced() => 
            new Vector2(Image.Border.left + Image.Border.right, Image.Border.top + Image.Border.bottom);

        private Vector2 MeasureStretched() => Image?.Size.ToVector2() ?? Vector2.Zero;

        protected override Vector2 MeasureOverride (in Vector2 available)
        {
            if (null == Image)
                return Vector2.Zero;

            switch (DrawMode)
            {
                case SpriteDrawMode.Auto:
                    if (Image != null && !Image.Border.IsZero)
                        return MeasureSliced();
                    else
                        return MeasureStretched();

                case SpriteDrawMode.Stretched:
                    return MeasureStretched();

                case SpriteDrawMode.Sliced:
                case SpriteDrawMode.SlicedNoFill:
                    return MeasureSliced();

                default:
                    throw new NotImplementedException();
            }
        }

        public override void Draw(GraphicsContext gc)
        {
            if (_meshInvalid)
                UpdateMesh();

            if (_quads == null)
                return;

            gc.Color = Color;
            gc.Image = Image;
            gc.MaskMode = MaskMode;
            gc.SortOrder = gc.SortOrder;
            gc.SortLayer = gc.SortLayer;
            gc.Draw(_quads, 0, _quads.Length);
        }

        void UpdateMesh()
        {
            switch (DrawMode)
            {
                case SpriteDrawMode.Auto:
                {
                    if (Image != null && !Image.Border.IsZero)
                        BuildSlicedMesh(true);
                    else
                        BuildStretchedMesh();
                    break;
                }

                case SpriteDrawMode.Stretched:
                    BuildStretchedMesh();
                    break;

                case SpriteDrawMode.Sliced:
                    BuildSlicedMesh(true);
                    break;

                case SpriteDrawMode.SlicedNoFill:
                    BuildSlicedMesh(false);
                    break;

                default:
                    throw new NotImplementedException();
            }

            _meshInvalid = false;
        }

        private void BuildStretchedMesh()
        {
            if (_quads == null || _quads.Length != 1)
                _quads = new Quad[1];

            var l = Rect.x;
            var t = Rect.y;
            var r = Rect.x + Rect.width;
            var b = Rect.y + Rect.height;

            _quads[0] = new Quad
            {
                TL = new Vertex { XY = new Vector2(l, t), UV = Vector2.Zero, Color = Color.White },
                TR = new Vertex { XY = new Vector2(r, t), UV = Vector2.OneZero, Color = Color.White },
                BR = new Vertex { XY = new Vector2(r, b), UV = Vector2.One, Color = Color.White },
                BL = new Vertex { XY = new Vector2(l, b), UV = Vector2.ZeroOne, Color = Color.White }
            };
        }

        private Vector2[] CalcOffsets(float borderMin, float borderMax, float imageSize, float rectMin, float rectSize, out int fill)
        {
            var uvMin = borderMin / imageSize;
            var uvMax = 1f - borderMax / imageSize;
            fill = -1;
            if (borderMin > 0 && borderMax > 0)
            {
                if (borderMin + borderMax > rectSize)
                {
                    borderMax = borderMax / (borderMax + borderMin) * rectSize;
                    borderMin = borderMin / (borderMax + borderMin) * rectSize;
                }
                fill = 1;
                return new Vector2[] {
                    new Vector2(rectMin, 0),
                    new Vector2(rectMin + borderMin, uvMin),
                    new Vector2(rectMin + rectSize - borderMax, uvMax),
                    new Vector2(rectMin + rectSize, 1)
                };
            }
            else if (borderMin >= rectSize)
            {
                return new Vector2[] {
                    new Vector2(rectMin, 0),
                    new Vector2(rectMin + rectSize, uvMin)
                };
            }
            else if (borderMin > 0)
            {
                fill = 1;
                return new Vector2[] {
                    new Vector2(rectMin, 0),
                    new Vector2(rectMin + borderMin, uvMin),
                    new Vector2(rectMin + rectSize, 1)
                };
            }
            else if (borderMax >= rectSize)
            {
                return new Vector2[] {
                    new Vector2(rectMin, uvMax),
                    new Vector2(rectMin + rectSize, 1)
                };
            }
            else if (borderMax > 0)
            {
                fill = 0;
                return new Vector2[] {
                    new Vector2(rectMin, 0),
                    new Vector2(rectMin + rectSize - borderMax, uvMin),
                    new Vector2(rectMin + rectSize, 1)
                };
            }
            else
            {
                return new Vector2[] {
                    new Vector2(rectMin, 0),
                    new Vector2(rectMin + rectSize, 1)
                };
            }
        }

        private void BuildSlicedMesh(bool filled)
        {
            _quads = null;
#if false
            var xoffsets = CalcOffsets(Image.Border.left, Image.Border.right, Image.Width, Rect.x, Rect.width, out var xfill);
            var yoffsets = CalcOffsets(Image.Border.top, Image.Border.bottom, Image.Height, Rect.y, Rect.height, out var yfill);

            int maxVerts = xoffsets.Length * yoffsets.Length;
            int maxTris = (xoffsets.Length - 1) * (yoffsets.Length - 1) * 2;

            if (filled)
            {
                xfill = -1;
                yfill = -1;
            }
            else
            {
                if (xfill < 0 && yfill >= 0)
                    xfill = 0;
                if (yfill < 0 && xfill >= 0)
                    yfill = 0;
                if (xfill >= 0)
                    maxTris -= 2;
            }

            if (_vertexBuffer == null || _vertexBuffer.Length != maxVerts)
                _vertexBuffer = new Vertex[maxVerts];

            if (_indexBuffer == null || _indexBuffer.Length != maxVerts)
                _indexBuffer = new short[maxTris * 3];

            // Populate the verts.
            var vertIndex = 0;
            for (int y = 0; y < yoffsets.Length; y++)
                for (int x = 0; x < xoffsets.Length; x++)
                    _vertexBuffer[vertIndex++] = new Vertex
                    {
                        XY = new Vector2(xoffsets[x].x, yoffsets[y].x),
                        UV = new Vector2(xoffsets[x].y, yoffsets[y].y),
                        Color = Color.White
                    };

            // Populate the triangles
            int triangleIndex = 0;
            for (int y = 0; y < yoffsets.Length - 1; y++)
            {
                int v = y * xoffsets.Length;
                int vn = v + xoffsets.Length;
                for (int x = 0; x < xoffsets.Length - 1; x++, v++, vn++)
                {
                    if (xfill == x && yfill == y)
                        continue;
                    _indexBuffer[triangleIndex++] = (short)v;
                    _indexBuffer[triangleIndex++] = (short)(v + 1);
                    _indexBuffer[triangleIndex++] = (short)vn;
                    _indexBuffer[triangleIndex++] = (short)(v + 1);
                    _indexBuffer[triangleIndex++] = (short)(vn + 1);
                    _indexBuffer[triangleIndex++] = (short)vn;
                }
            }
#endif
        }

        /// <summary>
        /// Route the sprites's pivot value through to the base node
        /// </summary>
        protected override Vector2 GetPivot() => Pivot;

        /// <summary>
        /// Invalidate the mesh when the rect has changed.
        /// </summary>
        /// <param name="old"></param>
        protected override void OnRectChanged (in Rect old)
        {
            base.OnRectChanged(old);
            _meshInvalid = true;
        }
    }
}
