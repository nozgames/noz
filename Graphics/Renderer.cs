/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

using SDL3;
using System.Numerics;
using System.Collections.Generic;

namespace NoZ.Graphics
{
    /// <summary>
    /// Singleton renderer for both 2D and 3D mesh rendering using SDL3 GPU API
    /// </summary>
    public sealed class Renderer
    {
        private static readonly Renderer _instance = new Renderer();
        public static Renderer Instance => _instance;

        // SDL3 GPU handles (replace nint with actual types if available)
        private nint _gpuDevice;
        private nint _gpuQueue;
        private nint _currentRenderPass;
        private nint _window;

        internal nint Device => _gpuDevice;

        // List of meshes to render this frame
        private readonly List<(Mesh mesh, Vector2 position)> _meshQueue = new();

        public Renderer() 
        { 
        }

        public unsafe void Load(nint window)
        {
            // TODO: Acquire GPU device and queue from SDL3
            _gpuDevice = SDL.CreateGPUDevice(SDL.GPUShaderFormat.DXIL, true, null);
            SDL.ClaimWindowForGPUDevice(_gpuDevice, _window);

            var targetInfo = stackalloc SDL.GPUColorTargetDescription[1]
            {
                new SDL.GPUColorTargetDescription
                {
                    Format =  SDL.GetGPUSwapchainTextureFormat(_gpuDevice, _window)
                }
            };

            //SDL.CreateGPUGraphicsPipeline(_gpuDevice, new SDL.GPUGraphicsPipelineCreateInfo
            //{
            //    TargetInfo = new SDL.GPUGraphicsPipelineTargetInfo
            //    {   
            //        NumColorTargets = 1,
            //        ColorTargetDescriptions = (nint)targetInfo
            //    },
            //    PrimitiveType = SDL.GPUPrimitiveType.TriangleList
            //}); 
        }

        public void Unload()
        {
            SDL.ReleaseWindowFromGPUDevice(_gpuDevice, _window);
            SDL.DestroyGPUDevice(_gpuDevice);
        }

        public void BeginFrame()
        {
            // TODO: Set up render pass descriptor and begin render pass
            // SDL.GPURenderPassDescriptor passDesc = ...;
            // _currentRenderPass = SDL.GPURenderPassEncoderBegin(_gpuQueue, ref passDesc);
        }

        public void DrawMesh(Mesh mesh, Material material, Matrix4x4 transform)
        {
            // TODO: Bind pipeline, set uniforms, bind vertex/index buffers, bind textures
            // TODO: Issue draw call for mesh using _currentRenderPass
        }

        public void DrawMesh(Mesh mesh, Vector2 position)
        {
            // Add mesh and position to the queue for this frame
            _meshQueue.Add((mesh, position));
        }

        public void EndFrame()
        {
            // TODO: End render pass, submit and present
            // SDL.GPURenderPassEncoderEnd(_currentRenderPass);
            // SDL.GPUQueueSubmit(_gpuQueue);
            // SDL.GPUQueuePresent(_gpuQueue);

            // Clear the queue after rendering
            _meshQueue.Clear();
        }
    }
}
