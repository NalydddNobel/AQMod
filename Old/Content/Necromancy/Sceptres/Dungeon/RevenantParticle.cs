using Aequus.Core.Graphics;
using Aequus.Core.Particles;
using System;
using Terraria.GameContent;

namespace Aequus.Old.Content.Particles;

public class RevenantParticle : ParallelParticleArray<RevenantParticle.Particle> {
    public override int ParticleCount => 600;

    public override void Draw(SpriteBatch spriteBatch) {
        spriteBatch.BeginDusts();

        Texture2D texture = AequusTextures.CorruptionSceptreProj.Value;
        Rectangle frame = texture.Bounds;
        Vector2 origin = frame.Size() / 2f;
        Texture2D bloomTexture = AequusTextures.BloomStrong;
        Vector2 bloomOrigin = bloomTexture.Size() / 2f;
        lock (this) {
            for (int k = 0; k < Particles.Length; k++) {
                Particle particle = Particles[k];

                if (particle == null || !particle.Active) {
                    continue;
                }

                float rotation = particle.Rotation;
                float scale = particle.Scale * particle.Opacity;
                Vector2 drawLocation = particle.Location - Main.screenPosition;
                Vector2 velocity = particle.Velocity;
                spriteBatch.Draw(bloomTexture, drawLocation, null, particle.BloomColor * particle.Opacity, 0f, bloomOrigin, particle.BloomScale * scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(texture, drawLocation, frame, particle.Color * particle.Opacity, rotation, origin, scale, SpriteEffects.None, 0f);
            }
            for (int k = 0; k < Particles.Length; k++) {
                Particle particle = Particles[k];

                if (particle == null || !particle.Active) {
                    continue;
                }

                float rotation = particle.Rotation;
                float scale = particle.Scale * particle.Opacity;
                Vector2 drawLocation = particle.Location - Main.screenPosition;
                Vector2 velocity = particle.Velocity;
                spriteBatch.Draw(texture, drawLocation, frame, Color.Black * (particle.Color.A / 255f) * particle.Opacity, rotation, origin, scale * 0.8f, SpriteEffects.None, 0f);
            }
        }

        spriteBatch.End();
    }

    protected override void UpdateParallel(int start, int end) {
        for (int i = start; i < end; i++) {
            Particle particle = Particles[i];
            if (particle == null || !particle.Active) {
                continue;
            }
            Active = true;

            particle.Velocity *= 0.96f;
            float velo = particle.Velocity.Length();
            particle.Rotation += velo * 0.0314f;

            float wantedOpacity = Math.Min(particle.Scale, 1f);
            if (particle.Opacity < wantedOpacity) {
                particle.Opacity += 0.1f;
            }
            else {
                particle.Opacity = wantedOpacity;
                particle.Scale -= 0.02f - velo / 1000f;
            }
            if (particle.Scale <= 0.1f || float.IsNaN(particle.Scale)) {
                particle.Active = false;
                continue;
            }

            if (!particle.dontEmitLight) {
                Lighting.AddLight(particle.Location, particle.Color.ToVector3() * 0.5f);
            }

            particle.Location += particle.Velocity;
        }
    }

    public override void OnActivate() {
        DrawLayers.Instance.PostDrawDust += Draw;
    }

    public override void Deactivate() {
        DrawLayers.Instance.PostDrawDust -= Draw;
    }

    public class Particle : IParticle {
        private bool _active;
        public bool Active {
            get => _active;
            set {
                if (!_active) {
                    Opacity = 0f;
                }
                _active = value;
            }
        }

        public Vector2 Location;
        public Vector2 Velocity;

        public float Rotation;
        public float Scale;
        public float Opacity;

        public Color Color;

        public Color BloomColor;
        public float BloomScale = 1f;

        public bool dontEmitLight;
    }
}
