/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

using NoZ.Audio;
using NoZ.Graphics;
using NoZ.VFX;
using SDL3;
using System.Drawing.Imaging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Advanced;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace NoZ
{
    public static class ResourceDatabase
    {
        private const int DefaultFontSize = 64;
        private const int SdfFontSize = 64;
        private static readonly string s_path = Path.Combine(Environment.CurrentDirectory, "resources");

        public static event Action<Resource>? ResourceLoaded;

        public static void Initialize() { }
        public static void Shutdown() { }

        public static void UnloadUnused()
        {
            Resource<VfxGraph>.UnloadUnused();
            Resource<Sprite>.UnloadUnused();
            Resource<SpriteAtlas>.UnloadUnused();
            Resource<Font>.UnloadUnused();
            Resource<AudioShader>.UnloadUnused();
            Resource<Shader>.UnloadUnused();
            Resource<Texture>.UnloadUnused();
        }

        public static string GetFullResourcePath(string path) =>
            Path.Combine(s_path, path.Replace('/', Path.DirectorySeparatorChar));

        public static SpriteAtlas LoadSpriteAtlas(string path)
        {
            if (Resource<SpriteAtlas>.TryGet(path, out var spriteAtlas))
                return spriteAtlas!;

            return Resource<SpriteAtlas>.Register(
                path,
                new SpriteAtlas(
                    SpriteAtlasSerializer.Deserialize(File.ReadAllText(GetFullResourcePath(path)! + ".json"))!,
                    LoadTexture(GetFullResourcePath(path)!)));
        }

        public static Sprite LoadSprite(string path)
        {
            if (Resource<Sprite>.TryGet(path, out var resource))
                return resource!;

            using var atlas = LoadSpriteAtlas(Path.GetDirectoryName(path)!);
            return atlas.LoadSprite(Path.GetFileName(path));
        }

        public static Shader LoadShader(string vs, string fs)
        {
            var path = vs + "_" + fs;
            if (Resource<Shader>.TryGet(path, out var resource))
                return resource!;

            vs = vs + ".vert";
            fs = fs + ".frag";

            var fullVertexPath = GetFullResourcePath(vs)!;
            var fullFragmentPath = GetFullResourcePath(fs)!;            
            var vertexSource = File.ReadAllText(fullVertexPath);
            var fragmentSource = File.ReadAllText(fullFragmentPath);
            var shader = Shader.Create(vertexSource, vs, fragmentSource, fs);

            // TODO: Implement SDL3 shader loading logic here
            return Resource<Shader>.Register(path, shader);
        }

        public static AudioShader LoadAudioShader(string path)
        {
            if (Resource<AudioShader>.TryGet(path, out var resource))
                return resource!;

            var jsonPath = GetFullResourcePath(path) + ".json";
            if (!File.Exists(jsonPath))
            {
                var wavPath = GetFullResourcePath(path) + ".wav";
                if (!File.Exists(wavPath))
                    throw new System.IO.FileNotFoundException(path);

                // TODO: Implement SDL3 audio loading logic here
                return Resource<AudioShader>.Register(path, new AudioShader());
            }

            var text = File.ReadAllText(path + ".json");
            var json = AudioShaderSerializer.Deserialize(text);

            if (json == null)
                throw new System.InvalidOperationException("Invalid audio shader");

            if (json.Sounds != null && json.Sounds.Length > 0)
                return Resource<AudioShader>.Register(path, new AudioShader());

            if (json.Sound != null)
                return Resource<AudioShader>.Register(path, new AudioShader());

            throw new System.InvalidOperationException("Missing sound!");
        }
        
        public unsafe static Font LoadFont(string path)
        {
            if (Resource<Font>.TryGet(path, out var resource))
                return resource!;

            // TODO: Implement SDL3 font loading logic here
            return Resource<Font>.Register(path, new Font(DefaultFontSize));
        }

        public static VfxGraph LoadVfxGraph(string path)
        {
            if (Resource<VfxGraph>.TryGet(path, out var resource))
                return resource!;

            var text = File.ReadAllText(GetFullResourcePath(path) + ".json");
            // TODO: Implement SDL3 VFX graph deserialization logic here
            return Resource<VfxGraph>.Register(path, VfxGraphDeserializer.Deserialize(text));
        }

        public static Texture LoadTexture(string path)
        {
            if (Resource<Texture>.TryGet(path, out var resource))
                return resource!;

            path = GetFullResourcePath(path)!;

            // Use ImageSharp to load PNG/JPG cross-platform
            using var image = SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgba32>(path);
            int width = image.Width;
            int height = image.Height;

            if (!image.Frames.RootFrame.DangerousTryGetSinglePixelMemory(out var memory))
                throw new Exception($"Failed to get pixel memory from image: {path}");

            unsafe
            {
                fixed (void* pointer = memory.Span)
                {
                    var texture = SDL.CreateGPUTexture(Application.Renderer.Device, new SDL.GPUTextureCreateInfo
                    {
                        Type = SDL.GPUTextureType.Texturetype2D,
                        Format = SDL.GPUTextureFormat.R8G8B8A8Uint,
                        Usage = SDL.GPUTextureUsageFlags.GraphicsStorageRead,
                        Width = (uint)width,
                        Height = (uint)height,
                        NumLevels = 1,
                        LayerCountOrDepth = 1
                    });

                    var transfer_buffer = SDL.CreateGPUTransferBuffer(Application.Renderer.Device, new SDL.GPUTransferBufferCreateInfo
                    {
                        Size = (uint)(width * height * 4),
                    });

                    var transferSource = SDL.MapGPUTransferBuffer(Application.Renderer.Device, transfer_buffer, false);
                    Unsafe.CopyBlockUnaligned((void*)transferSource, pointer, (uint)(width * height * 4));
                    SDL.UnmapGPUTransferBuffer(Application.Renderer.Device, transfer_buffer);

                    var commandBuffer = SDL.AcquireGPUCommandBuffer(Application.Renderer.Device);
                    var copyPass = SDL.BeginGPUCopyPass(commandBuffer);
                    SDL.UploadToGPUTexture(copyPass, new SDL.GPUTextureTransferInfo
                    {
                        TransferBuffer = transfer_buffer,
                    },
                    new SDL.GPUTextureRegion
                    {
                        Texture = texture,
                        W = (uint)width,
                        H = (uint)height,
                        D = 1
                    },
                    false);

                    return Resource<Texture>.Register(path, new Texture(texture, width, height));
                }
            }
        }
    }
}
