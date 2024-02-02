using Aequus.Core.Graphics;
using Aequus.Core.Particles;

namespace Aequus.Old.Content.Particles;

public class LegacyBloomParticle : ParallelParticleArray<LegacyBloomParticle.Particle> {
    public override int ParticleCount => 2000;

    public override void Draw(SpriteBatch spriteBatch) {
        spriteBatch.BeginDusts();

        Texture2D texture = AequusTextures.BaseParticleTexture;
        Texture2D bloomTexture = AequusTextures.BloomStrong;
        Vector2 bloomOrigin = bloomTexture.Size() / 2f;
        lock (this) {
            for (int k = 0; k < Particles.Length; k++) {
                Particle particle = Particles[k];

                if (particle == null || !particle.Active) {
                    continue;
                }

                Rectangle frame = texture.Frame(verticalFrames: 3, frameY: particle.Frame);
                Vector2 origin = frame.Size() / 2f;

                float rotation = particle.Rotation;
                float scale = particle.Scale;
                Vector2 drawLocation = particle.Location - Main.screenPosition;
                Vector2 velocity = particle.Velocity;
                spriteBatch.Draw(bloomTexture, drawLocation, null, particle.BloomColor, 0f, bloomOrigin, particle.BloomScale * scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(texture, drawLocation, frame, particle.Color, rotation, origin, scale, SpriteEffects.None, 0f);
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

            particle.Velocity *= 0.9f;
            float velo = particle.Velocity.Length();
            particle.Rotation += velo * 0.0314f;
            particle.Scale -= 0.05f - velo / 1000f;
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
                    Opacity = 1f;
                    Frame = (byte)Main.rand.Next(3);
                }
                _active = value;
            }
        }

        public Vector2 Location;
        public Vector2 Velocity;

        public float Rotation;
        public float Scale;
        public float Opacity;

        public byte Frame;

        public Color Color;

        public Color BloomColor;
        public float BloomScale = 1f;

        public bool dontEmitLight;
    }
}
