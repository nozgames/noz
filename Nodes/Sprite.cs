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
using System.Collections.Generic;
using System.IO;
using NoZ.Graphics;

namespace NoZ
{
    public enum SpriteDrawMode
    {
        Auto,
        Stretched,
        Sliced,
        SlicedNoFill
    }

    public class SpriteAnimation : Resource
    {
        public List<Image> Frames { get; private set; } = new List<Image>();
        public int FramesPerSecond = 30;
        public bool Looping { get; set; } = true;

        public SpriteAnimation(string name) : base (name)
        {
        }

        public static SpriteAnimation Create(string name, BinaryReader reader)
        {
            var anim = new SpriteAnimation(name);
            anim.FramesPerSecond = reader.ReadInt32();
            anim.Looping = reader.ReadBoolean();
            var frameCount = reader.ReadInt32();
            anim.Frames.Capacity = frameCount;
            for (int i = 0; i < frameCount; i++)
                anim.Frames.Add(ResourceDatabase.Load<Image>(reader.ReadString()));

            return anim;
        }
    }

    public class Sprite : Node, IDrawable
    {
        private Vertex[] _vertexBuffer;
        private short[] _indexBuffer;
        private SpriteAnimation _animation;
        private int _animationFrame = -1;
        private float _animationTime;
        private Image _image;

        /// <summary>
        /// True if the vertex buffer and index buffer need to be updated.
        /// </summary>
        private bool _meshInvalid = true;

        public Color Color { get; set; } = Color.White;

        public SpriteDrawMode DrawMode { get; set; } = SpriteDrawMode.Auto;

        public int SortOrder { get; set; }

        /// <summary>
        /// True if the sprite has an active SpriteAnimation
        /// </summary>
        public bool IsAnimating => _animationFrame >= 0;

        int IDrawable.SortOrder => SortOrder;

        private Vector2 _size;
        private Vector2 _pivot = Vector2.Half;

        public Vector2 Size {
            get => _size;
            set {
                _size = value;
                InvalidateRect();
            }
        }
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
        public Image Image {
            get => _image;
            set {
                if (_image == value)
                    return;

                Animation = null;
                _image = value;
                InvalidateRect();
            }
        }

        /// <summary>
        /// Current animation playing on the sprite
        /// </summary>
        public SpriteAnimation Animation {
            get => _animation;
            set {
                var hadAnimation = _animation != null;

                _animation = value;
                _animationFrame = 0;
                _animationTime = 0;

                var hasAnimation = _animation != null;
                if(hasAnimation)
                {
                    _image = _animation.Frames[0];
                }

                if(hasAnimation && !hadAnimation && Scene != null)
                    Scene.Subscribe(Scene.UpdateEvent, OnAnimationUpdate);
                else if (!hasAnimation && hadAnimation && Scene != null)
                {
                    Scene.Unsubscribe(Scene.UpdateEvent, this);
                    _animationFrame = -1;
                }                    
            }
        }

        public float AnimationSpeed { get; set; } = 1.0f;

        public MaskMode MaskMode { get; set; } = MaskMode.Inside;


        public Sprite ( )
        {
        }

        public Sprite (Image image)
        {
            _image = image;
            Size = Image.Size.ToVector2();
        }

        public Sprite (Image image, Color color)
        {
            _image = image;
            Color = color;
            Size = Image.Size.ToVector2();
        }

        public Sprite (Image image, Color color, SpriteDrawMode drawMode)
        {
            _image = image;
            Color = color;
            DrawMode = drawMode;
            Size = Image.Size.ToVector2();
        }

        private Vector2 MeasureSliced()
        {
            return new Vector2(Image.Border.left + Image.Border.right, Image.Border.top + Image.Border.bottom);
        }

        private Vector2 MeasureStretched() => Size;

        public override Vector2 Measure (in Vector2 available)
        {
            Vector2 size = Vector2.Zero;
            if (null != Image)
            {
                switch (DrawMode)
                {
                    case SpriteDrawMode.Auto:
                    {
                        if (Image != null && !Image.Border.IsZero)
                            size = MeasureSliced();
                        else
                            size = MeasureStretched();
                        break;
                    }

                    case SpriteDrawMode.Stretched:
                        size = MeasureStretched();
                        break;

                    case SpriteDrawMode.Sliced:
                    case SpriteDrawMode.SlicedNoFill:
                        size = MeasureSliced();
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
            return Vector2.Max(base.Measure(available), size);
        }

        bool IDrawable.Draw(GraphicsContext gc)
        {
            if (_meshInvalid)
                UpdateMesh();

            gc.SetColor(Color);
            gc.SetImage(Image);
            //gc.SetTransform(transform);
            gc.SetMaskMode(MaskMode);

            if (_indexBuffer == null)
                gc.Draw(PrimitiveType.TriangleStrip, _vertexBuffer, _vertexBuffer.Length);
            else
                gc.Draw(PrimitiveType.TriangleList, _vertexBuffer, _vertexBuffer.Length, _indexBuffer, _indexBuffer.Length);

            return true;
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
            if (_vertexBuffer == null || _vertexBuffer.Length != 4)
                _vertexBuffer = new Vertex[4];

            _indexBuffer = null;

            float l = Rect.x;
            float t = Rect.y;
            float r = Rect.x + Rect.width;
            float b = Rect.y + Rect.height;

            _vertexBuffer[0] = new Vertex { XY = new Vector2(l, t), UV = Vector2.Zero, Color = Color.White };
            _vertexBuffer[1] = new Vertex { XY = new Vector2(r, t), UV = Vector2.OneZero, Color = Color.White };
            _vertexBuffer[2] = new Vertex { XY = new Vector2(l, b), UV = Vector2.ZeroOne, Color = Color.White };
            _vertexBuffer[3] = new Vertex { XY = new Vector2(r, b), UV = Vector2.One, Color = Color.White };
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
        }

        public override void Arrange(Rect frame)
        {
            // Update size to match the arranged rectangle
            Size = frame.Size;

            // Update position to reflect the new size
            Position = new Vector2(
                frame.x + frame.width * Pivot.x,
                frame.y + frame.height * Pivot.y);
        }

        protected override Rect CalculateRect() => new Rect(-_size * Pivot, _size);

        protected override void OnRectChanged (in Rect old)
        {
            base.OnRectChanged(old);
            _meshInvalid = true;
        }

        protected override void OnEnterScene(Scene scene)
        {
            base.OnEnterScene(scene);

            if (_animation != null && _animationFrame >= 0)
                Scene.Subscribe(Scene.UpdateEvent, OnAnimationUpdate);
        }

        protected override void OnLeaveScene(Scene leaving)
        {
            base.OnLeaveScene(leaving);

            // If animating then we need to unsubscribe
            if (IsAnimating)
                Scene.Unsubscribe(Scene.UpdateEvent, this);
        }

        private void OnAnimationUpdate ()
        {
            _animationTime += (Time.DeltaTime * AnimationSpeed);
            var timePerFrame = 1.0f / _animation.FramesPerSecond;
            var frameAdvance = (int)(_animationTime / timePerFrame);
            if (frameAdvance == 0)
                return;

            _animationTime -= frameAdvance * timePerFrame;
            _animationFrame += frameAdvance;
            if(_animation.Looping)
            {
                _animationFrame = (_animationFrame % _animation.Frames.Count);
                if (_animationFrame < 0)
                    _animationFrame = _animation.Frames.Count + _animationFrame;
            }
            else if (_animationFrame >= _animation.Frames.Count)
            {
                _animationFrame = -1;
                _image = _animation.Frames[_animation.Frames.Count - 1];
                Scene.Unsubscribe(Scene.UpdateEvent, this);
                return;
            }

            _image = _animation.Frames[_animationFrame];
        }
    }
}
