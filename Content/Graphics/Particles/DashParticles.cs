using Aequus.Core.Graphics;
using Aequus.Core.Particles;

namespace Aequus.Content.Graphics.Particles;

public class DashParticles : ParallelParticleArray<DashParticles.Particle> {
    public override int ParticleCount => 100;

    public override void Draw(SpriteBatch spriteBatch) {
        spriteBatch.BeginDusts();

        Texture2D texture = AequusTextures.DashParticles;
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
                Color color = LightHelper.GetLightColor(particle.Location) * 0.66f * particle.Opacity;

                for (int i = 0; i < 7; i++) {
                    spriteBatch.Draw(texture, drawLocation - velocity * i, frame, color * 0.1f, rotation, origin, scale, SpriteEffects.None, 0f);
                }
                spriteBatch.Draw(texture, drawLocation, frame, color, rotation, origin, scale, SpriteEffects.None, 0f);
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

            float opacity = particle.Opacity;
            if (opacity <= 0f) {
                particle.Active = false;
                continue;
            }

            //particle.Rotation = particle.Velocity.ToRotation() + MathHelper.Pi;
            particle.Location += particle.Velocity;
            particle.Velocity *= 0.85f;
            if (particle.Velocity.LengthSquared() <= 32f) {
                opacity *= 0.95f;
                opacity -= 0.01f;
                particle.Opacity = opacity; // Update opacity property
            }
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

    }
}
