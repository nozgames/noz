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
        private Raylib_cs.Mesh _mesh;
        private Material _material;
        private bool _meshDirty;
        private Vector2 _size;

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

        public Raylib_cs.Color Color { get; set; } = Raylib_cs.Color.White;

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
            var renderWidth = _size.X;
            var renderHeight = _size.Y;
            var texture = _texture;

            var vertices = (Vector3*)_mesh.Vertices;
            var z = 0;
            vertices[0] = new Vector3(-renderWidth * _pivot.X, -renderHeight * _pivot.Y, z);
            vertices[1] = new Vector3(renderWidth * (1.0f - _pivot.X), -renderHeight * _pivot.Y, z);
            vertices[2] = new Vector3(renderWidth * (1.0f - _pivot.X), renderHeight * (1.0f - _pivot.Y), z);
            vertices[3] = new Vector3(-renderWidth * _pivot.X, renderHeight * (1.0f - _pivot.Y), z);

            var uv = (Vector2*)_mesh.TexCoords;
            uv[0] = new Vector2(0, 0);
            uv[1] = new Vector2(1, 0);
            uv[2] = new Vector2(1, 1);
            uv[3] = new Vector2(0, 1);

            _material.Texture = texture;

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
