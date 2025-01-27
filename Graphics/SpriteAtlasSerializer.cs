/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

using System.Text.Json;
using System.Text.Json.Serialization;

namespace NoZ.Graphics
{
    internal static partial class SpriteAtlasSerializer
    {
        internal class Sprite
        {
            public required string Name { get; set; }
            public required int X { get; set; }
            public required int Y { get; set; }
            public required int Width { get; set; }
            public required int Height { get; set; }
            public float PivotX { get; set; }
            public float PivotY { get; set; }
        }

        internal class SpriteAtlas
        {
            public required Sprite[] Sprites { get; set; }
        }

        [JsonSourceGenerationOptions(WriteIndented = true)]
        [JsonSerializable(typeof(SpriteAtlas))]
        [JsonSerializable(typeof(Sprite))]
        [JsonSerializable(typeof(int))]
        [JsonSerializable(typeof(string))]
        internal partial class SpriteAtlasContext : JsonSerializerContext
        {
        }

        public static SpriteAtlas Deserialize(string text) =>
            JsonSerializer.Deserialize<SpriteAtlas>(text, (System.Text.Json.Serialization.Metadata.JsonTypeInfo<SpriteAtlas>)SpriteAtlasContext.Default.SpriteAtlas)!;
    }
}
