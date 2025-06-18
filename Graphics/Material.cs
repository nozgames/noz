/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

namespace NoZ.Graphics
{
    public class Material : Resource<Material>
    {
        // TODO: Implement SDL3 material logic as needed.
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

                // Stub for SDL3 migration
                // Implement shader assignment logic for SDL3 here
            }
        }

        public Texture Texture
        {
            get => _texture!;
            set
            {
                if (_texture == value)
                    return;

                _texture = value;

                // Stub for SDL3 migration
                // Implement texture assignment logic for SDL3 here
            }
        }

        public Material(Shader shader)
        {
            _shader = shader.GetRef();

            // Stub for SDL3 migration
            // Initialize material logic for SDL3 here

            _texture = null;
        }

        protected internal override void Unload()
        {
            // Stub for SDL3 migration
            // Implement material unloading logic for SDL3 here

            _texture?.Dispose();
            _texture = null;

            _shader?.Dispose();
            _shader = null;
        }
    }
}
