/*
  
    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

using System.Numerics;
using NoZ.Graphics;

namespace NoZ.Nodes
{
    public class Circle : Node2D, IRenderable
    {
        // TODO: Implement SDL3 node logic as needed.
        private Material _material;
        private bool _meshDirty = true;
        private float _radius;
        private int _segments;
        private Color _color;

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

        public Color Color
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

        public Circle(float radius, Color color, int segments = 32)
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

            // TODO: Implement SDL3 rendering logic
        }

        private void InitializeMesh()
        {
            _material = new Material(Renderer2D.SolidShader);
            _meshDirty = true;
        }

        private void UpdateMesh()
        {
            // TODO: Implement SDL3 mesh update logic
            _meshDirty = false;
        }
    }
}
