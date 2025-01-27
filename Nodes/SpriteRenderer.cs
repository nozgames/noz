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
        private Raylib_cs.Mesh _mesh;
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

        public Raylib_cs.Color Color { get; set; } = Raylib_cs.Color.White;

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

            Renderer2D.Render(_mesh, _material!, LocalToWorld, SortOrder);
        }

        private unsafe void InitializeMesh()
        {
            _material = new Material(Renderer2D.SpriteShader);

            _mesh = new Raylib_cs.Mesh(4, 2);
            _mesh.AllocTexCoords();
            _mesh.AllocVertices();
            _mesh.AllocIndices();
            _mesh.AllocColors();

            var indices = _mesh.Indices;
            indices[0] = 0;
            indices[1] = 2;
            indices[2] = 1;
            indices[3] = 0;
            indices[4] = 3;
            indices[5] = 2;

            _meshDirty = true;
        }

        private unsafe void UpdateMesh()
        {
            var ppuInv = Application.PixelsPerUnitInv;
            var renderWidth = _sprite!.SourceRect.Width * ppuInv;
            var renderHeight = _sprite!.SourceRect.Height * ppuInv;
            var texture = _sprite.Texture;
            var pivot = _sprite.Pivot;

            var vertices = (Vector3*)_mesh.Vertices;
            var z = 0;
            vertices[0] = new Vector3(-renderWidth * pivot.X, -renderHeight * pivot.Y, z);
            vertices[1] = new Vector3( renderWidth * (1.0f - pivot.X), -renderHeight * pivot.Y, z);
            vertices[2] = new Vector3( renderWidth * (1.0f - pivot.X), renderHeight * (1.0f - pivot.Y), z);
            vertices[3] = new Vector3(-renderWidth * pivot.X, renderHeight * (1.0f - pivot.Y), z);

            if (texture != null)
            {
                var uv = (Vector2*)_mesh.TexCoords;
                var rect = _sprite!.SourceRect;
                uv[0] = new Vector2(rect.MinX / (float)texture.Width, (rect.MinY / (float)texture.Height));
                uv[1] = new Vector2(rect.MaxX / (float)texture.Width, (rect.MinY / (float)texture.Height));
                uv[2] = new Vector2(rect.MaxX / (float)texture.Width, (rect.MaxY / (float)texture.Height));
                uv[3] = new Vector2(rect.MinX / (float)texture.Width, (rect.MaxY / (float)texture.Height));

                _material!.Texture = texture;
            }

            var colors = (Raylib_cs.Color*)_mesh.Colors;
            colors[0] = Color;
            colors[1] = Color;
            colors[2] = Color;
            colors[3] = Color;

            Raylib_cs.Raylib.UploadMesh(ref _mesh, false);

            _meshDirty = false;
        }
    }
}
