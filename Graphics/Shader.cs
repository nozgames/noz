/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

namespace NoZ.Graphics
{
    public class Shader : Resource<Shader>
    {
        internal Raylib_cs.Shader _shader;

        internal Shader(Raylib_cs.Shader shader)
        {
            _shader = shader;
        }

        protected internal override void Unload()
        {
            Raylib_cs.Raylib.UnloadShader(_shader);
        }

        public int GetLocation(string name) =>
            Raylib_cs.Raylib.GetShaderLocation(_shader, name);

        public int GetLocationAttrib(string name) =>
            Raylib_cs.Raylib.GetShaderLocationAttrib(_shader, name);

        public unsafe void SetValue(int index, float value) =>
            Raylib_cs.Raylib.SetShaderValue(_shader, index, &value, Raylib_cs.ShaderUniformDataType.Float);
    }
}
