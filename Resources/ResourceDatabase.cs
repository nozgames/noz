/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

using NoZ.Audio;
using NoZ.Graphics;
using NoZ.VFX;

namespace NoZ
{
    public static class ResourceDatabase
    {
        private const int DefaultFontSize = 64;
        private const int SdfFontSize = 64;
        private static readonly string s_path = Path.Combine(Environment.CurrentDirectory, "resources");

        public static event Action<Resource>? ResourceLoaded;


        public static void Initialize()
        {
        }

        public static void Shutdown()
        {
        }

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
            Path.Combine(s_path, path);

        public static SpriteAtlas LoadSpriteAtlas(string path)
        {
            // Already loaded?
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

        public static Shader LoadShader(string? vs, string? fs)
        {
            var path = (vs ?? "") + "_" + (fs ?? "");
            if (Resource<Shader>.TryGet(path, out var resource))
                return resource!;

            if (vs != null)
                vs = GetFullResourcePath(vs) + ".vert";
            
            if (fs != null)
                fs = GetFullResourcePath(fs) + ".frag";

            return Resource<Shader>.Register(path, new Shader(Raylib_cs.Raylib.LoadShader(vs, fs)));
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

                return Resource<AudioShader>.Register(path, new AudioShader(Raylib_cs.Raylib.LoadSound(wavPath)));
            }

            var text = File.ReadAllText(path + ".json");
            var json = AudioShaderSerializer.Deserialize(text);

            if (json == null)
                throw new System.InvalidOperationException("Invalid audio shader");

            if (json.Sounds != null && json.Sounds.Length > 0)
                return Resource<AudioShader>.Register(path, new AudioShader(json.Sounds, json.Volume, json.Pitch));

            if (json.Sound != null)
                return Resource<AudioShader>.Register(path, new AudioShader(json.Sound ?? path, json.Volume, json.Pitch));

            throw new System.InvalidOperationException("Missing sound!");
        }
        
        public unsafe static Font LoadFont(string path)
        {
            if (Resource<Font>.TryGet(path, out var resource))
                return resource!;

            // Load the font to render the SDF with 
            var fileSize = 0;
            var fileData = Raylib_cs.Raylib.LoadFileData(GetFullResourcePath(path) + ".ttf", ref fileSize);
            var font = new Raylib_cs.Font
            {
                BaseSize = 16,
                GlyphCount = 95,
                Glyphs = Raylib_cs.Raylib.LoadFontData(fileData, fileSize, DefaultFontSize, null, 95, Raylib_cs.FontType.Default)
            };

            var atlas = Raylib_cs.Raylib.GenImageFontAtlas(font.Glyphs, &font.Recs, 95, DefaultFontSize, 4, 0);
            font.Texture = Raylib_cs.Raylib.LoadTextureFromImage(atlas);
            Raylib_cs.Raylib.UnloadImage(atlas);

            // Create the SDF font
            var fontSDF = new Raylib_cs.Font
            {
                BaseSize = DefaultFontSize,
                GlyphCount = 95,
                Glyphs = Raylib_cs.Raylib.LoadFontData(fileData, fileSize, DefaultFontSize, null, 0, Raylib_cs.FontType.Sdf)
            };

            atlas = Raylib_cs.Raylib.GenImageFontAtlas(fontSDF.Glyphs, &fontSDF.Recs, 95, SdfFontSize, 0, 1);
            fontSDF.Texture = Raylib_cs.Raylib.LoadTextureFromImage(atlas);
            Raylib_cs.Raylib.UnloadImage(atlas);

            Raylib_cs.Raylib.UnloadFileData(fileData);
            Raylib_cs.Raylib.SetTextureFilter(fontSDF.Texture, Raylib_cs.TextureFilter.Bilinear);

            return Resource<Font>.Register(path, new Font(fontSDF));
        }

        public static VfxGraph LoadVfxGraph(string path)
        {
            if (Resource<VfxGraph>.TryGet(path, out var resource))
                return resource!;

            var text = File.ReadAllText(GetFullResourcePath(path) + ".json");
            var graph = AudioShaderSerializer.Deserialize(text);

            return Resource<VfxGraph>.Register(path, VfxGraphDeserializer.Deserialize(text));
        }

        private static Texture LoadTexture(string path)
        {
            if (Resource<Texture>.TryGet(path, out var resource))
                return resource!;

            path = GetFullResourcePath(path)!;

            var texture = Raylib_cs.Raylib.LoadTexture(path + ".png");
            Raylib_cs.Raylib.SetTextureWrap(texture, Raylib_cs.TextureWrap.Clamp);
            Raylib_cs.Raylib.SetTextureFilter(texture, Raylib_cs.TextureFilter.Bilinear);

            return Resource<Texture>.Register(path, new Texture(texture));
        }
    }
}
