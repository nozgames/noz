/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

using NoZ.Tools;
using SDL3;
using System;
using System.IO;

namespace NoZ.Graphics
{
    // SDL3 implementation for Shader
    public class Shader : Resource<Shader>
    {
        // Handle to the SDL GPU pipeline/shader object
        private nint _pipeline;
        
        public nint Pipeline => _pipeline;

        private unsafe Shader(nint pipeline, void* vertexShader, ulong vertexShaderSize, void* fragmentShader, ulong fragmentShaderSize)
        {
            _pipeline = pipeline;
        }

        protected internal override void Unload()
        {
            if (_pipeline != nint.Zero)
            {
                SDL.ReleaseGPUGraphicsPipeline(Application.Renderer.Device, _pipeline);
                _pipeline = nint.Zero;
            }
        }

        public int GetLocation(string name) => 0; // TODO: Implement if needed
        public int GetLocationAttrib(string name) => 0; // TODO: Implement if needed
        public unsafe void SetValue(int index, float value) { /* TODO: Implement uniform set */ }

        internal static unsafe Shader Create(string vertexSource, string vertexName, string fragmentSource, string fragmentName)
        {
            var vertexPtr = ShaderCross.CompileVertexShader(vertexSource, vertexName, out var vertexSize);
            var fragmentPtr = ShaderCross.CompileFragmentShader(fragmentSource, fragmentName, out var fragmentSize);
            return new Shader(0, vertexPtr, vertexSize, fragmentPtr, fragmentSize);
        }
    }
}
