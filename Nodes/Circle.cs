/*
  
    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

using System.Numerics;
using NoZ.Graphics;

namespace NoZ.Nodes
{
    public class Circle : Node2D, IRenderable
    {
        private Raylib_cs.Mesh _mesh;
        private Material _material;
        private bool _meshDirty = true;
        private float _radius;
        private int _segments;
        private Raylib_cs.Color _color;

        public float Radius
        {
            get => _radius;
            set
            {
                if (_radius == value) return;
                _radius = value;
                _meshDirty = true;
            }
        }

        public int Segments
        {
            get => _segments;
            set
            {
                if (_segments == value) return;
                _segments = value < 3 ? 3 : value;
                _meshDirty = true;
            }
        }

        public Raylib_cs.Color Color
        {
            get => _color;
            set
            {
                if (_color.Equals(value)) return;
                _color = value;
                _meshDirty = true;
            }
        }

        public int SortOrder { get; set; }

        public Shader Shader
        {
            get => _material.Shader;
            set => _material.Shader = value;
        }

        public Circle(float radius, Raylib_cs.Color color, int segments = 32)
        {
            _radius = radius;
            _color = color;
            _segments = segments < 3 ? 3 : segments;
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
            _material = new Material(Renderer2D.SolidShader);

            // +1 for center, +1 to close the loop
            _mesh = new Raylib_cs.Mesh(_segments + 2, _segments * 3);
            _mesh.AllocVertices();
            _mesh.AllocIndices();
            _mesh.AllocColors();

            _meshDirty = true;
        }

        private unsafe void UpdateMesh()
        {
            var vertices = (Vector3*)_mesh.Vertices;
            var colors = (Raylib_cs.Color*)_mesh.Colors;
            var indices = _mesh.Indices;

            // Center vertex
            vertices[0] = new Vector3(0, 0, 0);
            colors[0] = _color;

            // Perimeter vertices
            for (int i = 0; i <= _segments; i++)
            {
                float angle = (float)i / _segments * MathF.PI * 2.0f;
                float x = MathF.Cos(angle) * _radius;
                float y = MathF.Sin(angle) * _radius;
                vertices[i + 1] = new Vector3(x, y, 0);
                colors[i + 1] = _color;
            }

            // Indices for triangle fan
            var idx = 0;
            for (int i = 0; i < _segments; i++)
            {
                indices[idx++] = (ushort)(i + 2);
                indices[idx++] = (ushort)(i + 1);
                indices[idx++] = 0;
            }

            Raylib_cs.Raylib.UploadMesh(ref _mesh, false);
            _meshDirty = false;
        }
    }
}
