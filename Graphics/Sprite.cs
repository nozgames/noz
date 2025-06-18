/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

using System.Numerics;

namespace NoZ.Graphics
{
    public class Sprite : Resource<Sprite>
    {
        private Vector2 _uv;
        private Vector2 _st;
        private Vector2 _pivot;
        private Rect _sourceRect;

        private Texture _texture;

        public Rect SourceRect => _sourceRect;

        public Texture Texture => _texture;

        public Vector2 Pivot => _pivot;

        public Sprite(Texture source, in Rect sourceRect, Vector2 pivot)
        {
            _pivot = pivot;
            _sourceRect = sourceRect;
            _texture = source.GetRef();

            _uv = new Vector2(_sourceRect.X / (float)_texture.Width, _sourceRect.Y / (float)_texture.Height);
            _st = new Vector2(_sourceRect.Width / (float)_texture.Width, _sourceRect.Height / (float)_texture.Height);
        }

        public Sprite(Texture source, in Rect sourceRect) : this(source, sourceRect, Vector2.One * 0.5f)
        {
        }

        protected internal override void Unload()
        {
            _texture.Dispose();
            _texture = null;
        }

        // TODO: Stub out SDL3 migration logic in Sprite.cs.
    }
}