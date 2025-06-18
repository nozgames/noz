/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

using System.Numerics;
using System.Text.Json;
using NoZ.Graphics;

namespace NoZ.VFX
{
    internal static partial class VfxGraphDeserializer
    {
        internal static VfxGraph Deserialize(string text)
        {
            var json = JsonSerializer.Deserialize(text, VfxGraphContext.Default.VfxGraphJson);
            
            if (json?.Emitters == null || json.Emitters.Length == 0)
                throw new System.InvalidOperationException("Invalid VFX graph");

            var graph = new VfxGraph
            {
                _emitters = new VfxEmitterConfig[json.Emitters.Length],
                _atlas = ResourceDatabase.LoadSpriteAtlas(json.Atlas)!
            };

            using var atlas = ResourceDatabase.LoadSpriteAtlas(json.Atlas)!;

            for (var i=0; i<graph._emitters.Length; i++)
            {
                ref var emitterJson = ref json.Emitters[i];
                ref var particleJson = ref emitterJson.Particle;

                var animation = default(Animation);
                var sprite = !string.IsNullOrEmpty(particleJson.Sprite)
                    ? atlas.LoadSprite(particleJson.Sprite)
                    : null;

                // TODO: Remove Raylib_cs dependencies and implement SDL3 VFX deserialization logic as needed.

                graph._emitters[i] = new VfxEmitterConfig
                {
                    Rate = emitterJson.Rate,
                    Burst = emitterJson.Burst,
                    Duration = emitterJson.Duration,
                    Angle = emitterJson.Angle,
                    Min = emitterJson.Min ?? Vector3.Zero,
                    Max = emitterJson.Max ?? Vector3.Zero,
                    Particle = new VfxParticleConfig
                    {
                        Duration = particleJson.Duration,
                        StartSize = particleJson.StartSize ?? new VfxRandomRange { Min = 1, Max = 1 },
                        EndSize = particleJson.EndSize ?? particleJson.StartSize ?? new VfxRandomRange { Min = 1, Max = 1 },
                        StartSpeed = particleJson.StartSpeed ?? new VfxRandomRange { Min = 0, Max = 0 },
                        EndSpeed = particleJson.EndSpeed ?? particleJson.StartSpeed ?? new VfxRandomRange { Min = 0, Max = 0 },
                        StartColor = particleJson.StartColor ?? new VfxRandomColor { Min = Color.White, Max = Color.White },
                        EndColor = particleJson.EndColor ?? particleJson.StartColor ?? new VfxRandomColor { Min = Color.White, Max = Color.White },
                        Sprite = sprite,
                        Animation = animation,
                        AnimationSpeed = particleJson.AnimationSpeed ?? new VfxRandomRange { Min = 1, Max = 1 },
                        AnimationStartTime = particleJson.AnimationStartTime ?? new VfxRandomRange { Min = 0, Max = 0 },
                        AnimationLoop = particleJson.AnimationLoop ?? true,
                        SortOrder = particleJson.SortOrder,
                        StartRotation = particleJson.StartRotation ?? new VfxRandomRange { Min = 0, Max = 0 },
                        Rotation = particleJson.Rotation ?? new VfxRandomRange { Min = 0, Max = 0 },
                    }
                };
            }

            return graph;
        }
        
        private static void Unload(VfxGraph vfxGraph)
        {
        }
    }    
}