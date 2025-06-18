/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

using NoZ.Graphics;
using System.Numerics;

namespace NoZ.Nodes
{
    public class SpriteRenderer : Node2D, IRenderable
    {
        private Sprite? _sprite;
        private Material? _material;
        private bool _meshDirty;

        public int SortOrder { get; set; }
        
        public Shader Shader
        {
            get => _material.Shader;
            set => _material.Shader = value;
        }

        public Sprite? Sprite
        {
            get => _sprite;
            set
            {
                if (_sprite == value)
                    return;

                _sprite?.Dispose();
                _sprite = value.GetRef();
                _meshDirty = true;
            }
        }

        public Vector4 Color { get; set; } = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);

        // TODO: Implement SDL3 node logic as needed.
        // TODO: shader
        // TODO: shader params

        public SpriteRenderer()
        {
            InitializeMesh();
        }

        public SpriteRenderer(Sprite sprite)
        {
            Sprite = sprite;
            InitializeMesh();
        }

        public SpriteRenderer(string path)
        {
            _sprite = ResourceDatabase.LoadSprite(path);
            InitializeMesh();
        }


        public void Render()
        {
            if (_meshDirty)
                UpdateMesh();

            Renderer2D.Render(null, _material!, LocalToWorld, SortOrder);
        }

        private void InitializeMesh()
        {
            _material = new Material(Renderer2D.SpriteShader);

            // TODO: Initialize mesh logic for SDL3
            _meshDirty = true;
        }

        private void UpdateMesh()
        {
            // TODO: Update mesh logic for SDL3
            _meshDirty = false;
        }
    }
}
