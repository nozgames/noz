/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

namespace NoZ.Graphics
{
    // SDL3 stub for Shader
    public class Shader : Resource<Shader>
    {
        // TODO: Add SDL3 shader fields as needed
        public Shader() { }
        protected internal override void Unload() { }
        public int GetLocation(string name) => 0;
        public int GetLocationAttrib(string name) => 0;
        public unsafe void SetValue(int index, float value) { }
    }
}
