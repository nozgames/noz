/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

using System.Numerics;
using NoZ.Graphics;

namespace NoZ.Nodes
{
    public class Quad : Node2D, IRenderable
    {
        private Texture _texture;
        private Vector2 _pivot;
        private Material _material;
        private bool _meshDirty;
        private Vector2 _size;
        private Mesh _mesh = new Mesh();

        public Vector2 Size
        {
            get => _size;
            set
            {
                if (_size == value)
                    return;

                _size = value;
                _meshDirty = true;
            }
        }

        public int SortOrder { get; set; }

        public Shader Shader
        {
            get => _material.Shader;
            set => _material.Shader = value;
        }

        public Vector2 Pivot
        {
            get => _pivot;
            set
            {
                if (_pivot == value)
                    return;

                _pivot = value;
                _meshDirty = true;
            }
        }

        public Texture Texture
        {
            get => _texture;
            set
            {
                _texture = value;
                _meshDirty = true;
            }
        }

        public Color Color { get; set; } = Color.White;

        // TODO: Implement SDL3 node logic as needed.
        // TODO: shader
        // TODO: shader params

        public Quad()
        {
            _pivot = new Vector2(0.5f, 0.5f);
            InitializeMesh();
        }

        public void Render()
        {
            if (_meshDirty)
                UpdateMesh();

            Renderer2D.Render(_mesh, _material, LocalToWorld, SortOrder);
        }

        private unsafe void InitializeMesh()
        {
            _material = new Material(Renderer2D.SpriteShader);
            
            // TODO: Implement SDL3 node logic as needed.
            _meshDirty = true;
        }

        private unsafe void UpdateMesh()
        {
            var renderWidth = _size.X;
            var renderHeight = _size.Y;
            var texture = _texture;

            // TODO: Implement SDL3 node logic as needed.
            _material.Texture = texture;

            _meshDirty = false;
        }
    }
}
