/*

    Copyright (c) 2024 NoZ Games, LLC. All rights reserved.

*/

using System.Diagnostics;
using System.Numerics;
using NoZ.Helpers;
using Raylib_cs;

namespace NoZ.VFX
{
    /// <summary>
    /// Track and render all sprites in the game each frame
    /// </summary>
    public static class VfxSystem
    {
#if false
        private static EntityCollection<VfxEmitter>? _emitters;
        private static EntityCollection<VfxParticle>? _particles;

        public static void Initialize(int maxEmitters, int maxParticles)
        {
            _emitters = new EntityCollection<VfxEmitter>(maxEmitters);
            _particles = new EntityCollection<VfxParticle>(maxParticles);
        }

        public static void Shutdown()
        {
            _emitters?.Dispose();
            _particles?.Dispose();
            _emitters = null;
            _particles = null;
        }
        
        public static void Update()
        {
            Debug.Assert(_emitters != null);
            Debug.Assert(_particles != null);

            var emitters = _emitters!.Entities;
            for (int i = 0, c = emitters.Length; i < c; i++)
                UpdateEmitter(ref emitters[i]);
            
            var particles = _particles!.Entities;
            for (int i = 0, c = particles.Length; i < c; i++)
                UpdateParticle(ref particles[i]);
            
            _emitters.Collect();
            _particles.Collect();
        }

        private static void UpdateEmitter(ref VfxEmitter emitter)
        {
            Debug.Assert(_emitters != null);

            // Emit as many particles as we can
            emitter.Elapsed += Time.DeltaTime;
            if (emitter.Elapsed >= emitter.Duration)
            {
                _emitters.Destroy(emitter.GetId());
                return;
            }
            
            while (emitter.Elapsed > emitter.EmitRate)
            {
                emitter.Elapsed -= emitter.EmitRate;
                EmitParticle(ref emitter);
            }
        }
        
        private static void EmitParticle(ref VfxEmitter emitter) =>
            EmitParticle(ref emitter.Config, emitter.Transform.Position, Vector3.Zero);
        
        private static void EmitParticle(ref VfxEmitterConfig emitterConfig, in Vector3 position, in Vector3 direction)
        {
            Debug.Assert(_particles != null);

            ref var particleConfig = ref emitterConfig.Particle;
            ref var particle = ref _particles.Create().Value;
            particle.Transform.Position = position + Randomizer.Range(emitterConfig.Min, emitterConfig.Max);
            particle.Transform.Scale = 1.0f;
            particle.Duration = particleConfig.Duration.GetRandomValue();
            particle.DurationInv = 1.0f / particle.Duration;
            particle.Size = new Vector2(particleConfig.StartSize.GetRandomValue(), particleConfig.EndSize.GetRandomValue());
            particle.Sprite.Color = Color.White;
            particle.Sprite.Sprite = particleConfig.Sprite;
            particle.Sprite.Scale = Vector2.One;
            particle.Elapsed = 0.0f;
            particle.Direction = (particle.Transform.Position - position).LengthSquared() > float.Epsilon
                ? Vector3.Normalize(particle.Transform.Position - position)
                : Vector3.UnitY;
            particle.Speed = new Vector2(particleConfig.StartSpeed.GetRandomValue(), particleConfig.EndSpeed.GetRandomValue());
            particle.StartColor = particleConfig.StartColor.GetRandomValue();
            particle.EndColor = particleConfig.EndColor.GetRandomValue();
            particle.BaseSortOrder = particleConfig.SortOrder;

            var rotation = particleConfig.StartRotation.GetRandomValue();
            particle.Rotation = new Vector2(rotation, rotation + particleConfig.Rotation.GetRandomValue());

            // TODO: support configuration of loop, etc
            if (particleConfig.Animation.FrameCount > 0)
                AnimationHelper.Play(
                    ref particle.Animator,
                    ref particle.Sprite,
                    particleConfig.Animation,
                    particleConfig.AnimationLoop,
                    particleConfig.AnimationSpeed.GetRandomValue(),
                    particleConfig.AnimationStartTime.GetRandomValue());
        }

        private static void UpdateParticle(ref VfxParticle particle)
        {
            Debug.Assert(_particles != null);

            particle.Elapsed += Time.DeltaTime;
            if (particle.Elapsed >= particle.Duration)
            {
                _particles.Destroy(particle.GetId());
                return;
            }

            AnimationHelper.Update(ref particle.Animator, ref particle.Sprite);

            var t = particle.Elapsed * particle.DurationInv;
            particle.Sprite.Scale = Vector2.One * MathEx.Lerp(particle.Size.X, particle.Size.Y, t);
            particle.Transform.Position += particle.Direction * MathEx.Lerp(particle.Speed.X, particle.Speed.Y, t) * Time.DeltaTime;
            particle.Sprite.Color = MathEx.Lerp(particle.StartColor, particle.EndColor, t);
            particle.Sprite.Rotation = MathEx.Lerp(particle.Rotation.X, particle.Rotation.Y, t);
        }

        public static void Render()
        {
            var particles = _particles!.Entities;
            for (int i = 0, c = particles.Length; i < c; i++)
            {
                ref var particle = ref particles[i];
                ref var spriteRenderer = ref particle.Sprite;
                ref var transform = ref particle.Transform;
                SortOrderHelper.UpdateSortOrder(ref spriteRenderer, in transform.Position, particle.BaseSortOrder);
                //SpriteRenderer.Render(transform, spriteRenderer);
            }
        }


        public static void Play(Resource<VfxGraph>? graph, Vector3 position, Vector3 direction) =>
            Play(graph?.Value, position, direction);

        public static void Play(VfxGraph? graph, Vector3 position, Vector3 direction)
        {
            if (graph == null || graph == null)
                return;

            Debug.Assert(_emitters != null);

            var emitters = graph._emitters;
            for(var emitterIndex=0; emitterIndex<emitters!.Length; emitterIndex++)
            {
                // Handle any burst
                ref var emitterConfig = ref emitters[emitterIndex];
                var burstCount = emitterConfig.Burst.GetRandomValue();
                if (burstCount > 0)
                {
                    for (var burstIndex = 0; burstIndex < burstCount; burstIndex++)
                        EmitParticle(ref emitterConfig, position, direction);
                }
                
                // Skip emitters with a rate of 0
                if (emitterConfig.Rate == 0)
                    continue;
                
                ref var emitter = ref _emitters.Create().Value;
                emitter.Particle = emitterConfig.Particle;
                emitter.Transform.Position = position;
                emitter.EmitRate = 1.0f / emitterConfig.Rate;
                emitter.Duration = emitterConfig.Duration.GetRandomValue();
                emitter.Config = emitterConfig;
            }
        }
#endif
    }
}
