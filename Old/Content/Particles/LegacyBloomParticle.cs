using Aequus.Core.Graphics;
using Aequus.Core.Particles;

namespace Aequus.Old.Content.Particles;

public class LegacyBloomParticle : ParallelParticleSystem<LegacyBloomParticle.Particle> {
    public override System.Int32 ParticleCount => 2000;

    public override void Draw(SpriteBatch spriteBatch) {
        spriteBatch.BeginDusts();

        Texture2D texture = AequusTextures.BaseParticleTexture;
        Texture2D bloomTexture = AequusTextures.BloomStrong;
        Vector2 bloomOrigin = bloomTexture.Size() / 2f;
        lock (this) {
            for (System.Int32 k = 0; k < Particles.Length; k++) {
                Particle particle = Particles[k];

                if (particle == null || !particle.Active) {
                    continue;
                }

                Rectangle frame = texture.Frame(verticalFrames: 3, frameY: particle.Frame);
                Vector2 origin = frame.Size() / 2f;

                System.Single rotation = particle.Rotation;
                System.Single scale = particle.Scale;
                Vector2 drawLocation = particle.Location - Main.screenPosition;
                Vector2 velocity = particle.Velocity;
                spriteBatch.Draw(bloomTexture, drawLocation, null, particle.BloomColor, 0f, bloomOrigin, particle.BloomScale * scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(texture, drawLocation, frame, particle.Color, rotation, origin, scale, SpriteEffects.None, 0f);
            }
        }

        spriteBatch.End();
    }

    protected override void UpdateParallel(System.Int32 start, System.Int32 end) {
        for (System.Int32 i = start; i < end; i++) {
            Particle particle = Particles[i];
            if (particle == null || !particle.Active) {
                continue;
            }
            Active = true;

            particle.Velocity *= 0.9f;
            System.Single velo = particle.Velocity.Length();
            particle.Rotation += velo * 0.0314f;
            particle.Scale -= 0.05f - velo / 1000f;
            if (particle.Scale <= 0.1f || System.Single.IsNaN(particle.Scale)) {
                particle.Active = false;
                continue;
            }

            if (!particle.dontEmitLight) {
                Lighting.AddLight(particle.Location, particle.Color.ToVector3() * 0.5f);
            }

            particle.Location += particle.Velocity;
        }
    }

    public override void Activate() {
        DrawLayers.Instance.PostDrawDust += Draw;
    }

    public override void Deactivate() {
        DrawLayers.Instance.PostDrawDust -= Draw;
    }

    public class Particle : IParticle {
        private System.Boolean _active;
        public System.Boolean Active {
            get => _active;
            set {
                if (!_active) {
                    Opacity = 1f;
                    Frame = (System.Byte)Main.rand.Next(3);
                }
                _active = value;
            }
        }

        public Vector2 Location;
        public Vector2 Velocity;

        public System.Single Rotation;
        public System.Single Scale;
        public System.Single Opacity;

        public System.Byte Frame;

        public Color Color;

        public Color BloomColor;
        public System.Single BloomScale = 1f;

        public System.Boolean dontEmitLight;
    }
}
