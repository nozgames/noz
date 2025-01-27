/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

using System.Text.Json;
using System.Text.Json.Serialization;

namespace NoZ.Audio
{
    internal static partial class AudioShaderSerializer
    {
        internal class AudioShader
        {
            public string? Sound;
            public string[]? Sounds;
            public AudioRandomRange? Pitch;
            public AudioRandomRange? Volume;
        }

        [JsonSourceGenerationOptions(IncludeFields = true, Converters = [ typeof(AudioRandomRangeJsonConverter)])]
        [JsonSerializable(typeof(AudioShader))]
        [JsonSerializable(typeof(AudioRandomRange))]
        [JsonSerializable(typeof(int))]
        [JsonSerializable(typeof(string))]
        internal partial class AudioShaderContext : JsonSerializerContext
        {
        }

        private class AudioRandomRangeJsonConverter : JsonConverter<AudioRandomRange>
        {
            public override AudioRandomRange Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Number && reader.TryGetSingle(out var value))
                    return new AudioRandomRange { Min = value, Max = value };

                if (reader.TokenType == JsonTokenType.StartArray)
                {
                    reader.Read();
                    reader.TryGetSingle(out var min);
                    reader.Read();
                    reader.TryGetSingle(out var max);
                    reader.Read();
                    return new AudioRandomRange { Min = min, Max = max };
                }

                return default;
            }

            public override void Write(Utf8JsonWriter w, AudioRandomRange r, JsonSerializerOptions o) =>
                throw new System.NotImplementedException();
        }

        public static AudioShader Deserialize(string text) =>
            JsonSerializer.Deserialize(text, AudioShaderContext.Default.AudioShader);
    }
}
