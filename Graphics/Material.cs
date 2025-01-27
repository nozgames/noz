/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

namespace NoZ.Graphics
{
    public class Material : Resource<Material>
    {
        internal Raylib_cs.Material _material;
        private Shader? _shader;
        private Texture? _texture;

        public Shader Shader
        {
            get => _shader!;
            set
            {
                if (_shader == value) return;
                _shader?.Dispose();
                _shader = value.GetRef();

                _material.Shader = value != null ? value._shader : default;
            }
        }

        public unsafe Texture Texture
        {
            get => _texture!;
            set
            {
                if (_texture == value)
                    return;

                _texture = value;

                fixed (Raylib_cs.Material* mm = &_material)
                    Raylib_cs.Raylib.SetMaterialTexture(mm, Raylib_cs.MaterialMapIndex.Diffuse, _texture != null ? _texture._texture : default);
            }
        }

        public Material(Shader shader)
        {
            _shader = shader.GetRef();
            _material = Raylib_cs.Raylib.LoadMaterialDefault();
            _material.Shader = _shader._shader;
            _texture = null;
        }

        protected internal override void Unload()
        {
            Raylib_cs.Raylib.UnloadMaterial(_material);

            _texture?.Dispose();
            _texture = null;

            _shader?.Dispose();
            _shader = null;
        }
    }
}
