using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace NoZ.Tools
{
    public unsafe static class ShaderCross
    {
        public enum ShaderStage : int
        {
            Vertex,
            Fragment,
            Compute
        };

        public struct HLSL_Define
        {
            public char* Name;   /**< The define name. */
            public char* Value;  /**< An optional value for the define. Can be NULL. */
        }

        public struct HLSL_Info
        {
            public char* Source;
            public char* EntryPoint;
            public char* IncludePath;
            public HLSL_Define* Defines;
            public ShaderStage ShaderStage;
            public bool EnableDebug;
            public char* Name;
            public uint Props;
        }

        public static void Init()
        {
            SDL_ShaderCross_Init();
        }

        internal static unsafe void* CompileVertexShader(string source, string name, out ulong size) =>
            CompileShader(source, name, ShaderStage.Vertex, out size);

        internal static unsafe void* CompileFragmentShader(string source, string name, out ulong size) =>
            CompileShader(source, name, ShaderStage.Fragment, out size);

        private static unsafe void* CompileShader(string source, string name, ShaderStage stage, out ulong size)
        { 
            var sourcePtr = Marshal.StringToHGlobalAnsi(source);
            var namePtr = Marshal.StringToHGlobalAnsi(name);
            var entryPointPtr = Marshal.StringToHGlobalAnsi("main");

            var hlslInfo = new HLSL_Info
            {
                Source = (char*)sourcePtr,
                EntryPoint = (char*)entryPointPtr,
                IncludePath = null,
                Defines = null,
                ShaderStage = stage,
                EnableDebug = false,
                Name = (char*)namePtr,
                Props = 0
            };

            ulong bytecodeSize = 0;
            void* bytecode = SDL_ShaderCross_CompileDXILFromHLSL(&hlslInfo, &bytecodeSize);
            if (bytecode == null)
            {
                SDL3.SDL.LogError(SDL3.SDL.LogCategory.Application, $"Failed to compile shader: {SDL3.SDL.GetError()}");
            }

            Marshal.FreeHGlobal(sourcePtr);
            Marshal.FreeHGlobal(namePtr);
            Marshal.FreeHGlobal(entryPointPtr);

            size = bytecodeSize;
            return bytecode;
        }

        [DllImport("SDL3_shadercross", EntryPoint = "SDL_ShaderCross_Init")]
        private extern static bool SDL_ShaderCross_Init();

        [DllImport("SDL3_shadercross", EntryPoint = "SDL_ShaderCross_CompileDXILFromHLSL")]
        private extern static void* SDL_ShaderCross_CompileDXILFromHLSL(HLSL_Info* info, ulong* size);
    }
}
