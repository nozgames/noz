/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;
using NoZ.Graphics;
using Raylib_cs;

namespace NoZ.VFX
{
    internal static partial class VfxGraphDeserializer
    {
        private class VfxRandomRangeJsonConverter : JsonConverter<VfxRandomRange>
        {
            public override VfxRandomRange Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Number && reader.TryGetSingle(out var value))
                    return new VfxRandomRange { Min = value, Max = value };

                if (reader.TokenType == JsonTokenType.StartArray)
                {
                    reader.Read();
                    reader.TryGetSingle(out var min);
                    reader.Read();
                    reader.TryGetSingle(out var max);
                    reader.Read();
                    return new VfxRandomRange { Min = min, Max = max };
                }

                return default;
            }

            public override void Write(Utf8JsonWriter w, VfxRandomRange r, JsonSerializerOptions o) =>
                throw new System.NotImplementedException();
        }
        
        private class VfxRandomRangeIntJsonConverter : JsonConverter<VfxRandomRangeInt>
        {
            public override VfxRandomRangeInt Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out var value))
                    return new VfxRandomRangeInt { Min = value, Max = value };

                if (reader.TokenType == JsonTokenType.StartArray)
                {
                    reader.Read();
                    reader.TryGetInt32(out var min);
                    reader.Read();
                    reader.TryGetInt32(out var max);
                    reader.Read();
                    return new VfxRandomRangeInt { Min = min, Max = max };
                }

                return default;
            }

            public override void Write(Utf8JsonWriter w, VfxRandomRangeInt r, JsonSerializerOptions o) =>
                throw new System.NotImplementedException();
        }
        
        private class ColorJsonConverter : JsonConverter<Color>
        {
            public static Color ParseColor(string value)
            {
                // Parse the hex string into a color
                if (!value.StartsWith('#'))
                    return Color.White;
                    
                var hex = value[1..];
                if (hex.Length == 3)
                {
                    var r = Convert.ToByte(hex[..1], 16);
                    var g = Convert.ToByte(hex.Substring(1, 1), 16);
                    var b = Convert.ToByte(hex.Substring(2, 1), 16);
                    return new Color(r + (r << 4), g + (g << 4), b + (b << 4), (byte)255);
                }
                else if (hex.Length == 6)
                {
                    var r = Convert.ToByte(hex[..2], 16);
                    var g = Convert.ToByte(hex.Substring(2, 2), 16);
                    var b = Convert.ToByte(hex.Substring(4, 2), 16);
                    return new Color(r, g, b, (byte)255);
                }
                else if (hex.Length == 8)
                {
                    var r = Convert.ToByte(hex[..2], 16);
                    var g = Convert.ToByte(hex.Substring(2, 2), 16);
                    var b = Convert.ToByte(hex.Substring(4, 2), 16);
                    var a = Convert.ToByte(hex.Substring(6, 2), 16);
                    return new Color(r, g, b, a);
                }

                return Color.White;
            }
            
            public override Color Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.String)
                    return ParseColor(reader.GetString()!);

                return Color.White;
            }

            public override void Write(Utf8JsonWriter w, Color r, JsonSerializerOptions o) =>
                throw new System.NotImplementedException();
        }
        
        private class VfxRandomColorJsonConverter : JsonConverter<VfxRandomColor>
        {
            public override VfxRandomColor Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.String)
                {
                    var color = ColorJsonConverter.ParseColor(reader.GetString()!);
                    return new VfxRandomColor { Min = color, Max = color };
                }

                if (reader.TokenType == JsonTokenType.StartArray)
                {
                    reader.Read();
                    var min = ColorJsonConverter.ParseColor(reader.GetString()!);
                    reader.Read();
                    var max = ColorJsonConverter.ParseColor(reader.GetString()!);
                    reader.Read();
                    return new VfxRandomColor { Min = min, Max = max };
                }

                return new VfxRandomColor { Min = Color.White, Max = Color.White };
            }

            public override void Write(Utf8JsonWriter w, VfxRandomColor r, JsonSerializerOptions o) =>
                throw new System.NotImplementedException();
        }

        private class VfxVector3JsonConverter : JsonConverter<Vector3>
        {
            public override Vector3 Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.StartArray)
                {
                    reader.Read();
                    var x = reader.GetSingle();
                    reader.Read();
                    var y = reader.GetSingle();
                    reader.Read();
                    var z = reader.GetSingle();
                    reader.Read();
                    reader.Read();
                    return new Vector3(x, y, z);
                }

                return Vector3.Zero;
            }

            public override void Write(Utf8JsonWriter w, Vector3 r, JsonSerializerOptions o) =>
                throw new System.NotImplementedException();
        }
        private class VfxParticleJson
        {
            public string? Sprite;
            public required VfxRandomRange Duration; 
            public VfxRandomRange? StartSize;
            public VfxRandomRange? EndSize;
            public VfxRandomRange? StartSpeed;
            public VfxRandomRange? EndSpeed;
            public VfxRandomRange? StartRotation;
            public VfxRandomRange? Rotation;
            public int SortOrder;
            public VfxRandomColor? StartColor;
            public VfxRandomColor? EndColor;
            public AnimationConfig? Animation;
            public bool? AnimationLoop;
            public VfxRandomRange? AnimationSpeed;
            public VfxRandomRange? AnimationStartTime;
        }
        
        private class VfxEmitterJson
        {
            public int Rate;
            public VfxRandomRangeInt Burst;
            public VfxRandomRange Duration;
            public required VfxParticleJson Particle;
            public VfxRandomRange? X;
            public VfxRandomRange? Y;
            public Vector3? Min;
            public Vector3? Max;
            public VfxRandomRange Angle; 
        }

        private class VfxGraphJson
        {
            public required string Atlas;
            public required VfxEmitterJson[] Emitters;
        }

        [JsonSourceGenerationOptions(IncludeFields = true, Converters = [
            typeof(VfxRandomRangeJsonConverter),
            typeof(VfxRandomRangeIntJsonConverter),
            typeof(ColorJsonConverter),
            typeof(VfxRandomColorJsonConverter),
            typeof(VfxVector3JsonConverter)])]
        [JsonSerializable(typeof(VfxGraphJson))]
        [JsonSerializable(typeof(VfxEmitterJson))]
        [JsonSerializable(typeof(VfxParticleJson))]
        [JsonSerializable(typeof(VfxRandomRange))]
        [JsonSerializable(typeof(VfxRandomRangeInt))]
        [JsonSerializable(typeof(Color))]
        [JsonSerializable(typeof(int))]
        [JsonSerializable(typeof(string))]
        [JsonSerializable(typeof(float))]
        private partial class VfxGraphContext : JsonSerializerContext
        {
        }
    }    
}