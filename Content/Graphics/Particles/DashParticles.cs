using Aequus.Core.Graphics;
using Aequus.Core.Particles;

namespace Aequus.Content.Graphics.Particles;

public class DashParticles : ParallelParticleSystem<DashParticles.Particle> {
    public override System.Int32 ParticleCount => 100;

    public override void Draw(SpriteBatch spriteBatch) {
        spriteBatch.BeginDusts();

        Texture2D texture = AequusTextures.DashParticles;
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
                Color color = ExtendLight.Get(particle.Location) * 0.66f * particle.Opacity;

                for (System.Int32 i = 0; i < 3; i++) {
                    spriteBatch.Draw(texture, drawLocation - velocity * i, frame, color * 0.1f, rotation, origin, scale, SpriteEffects.None, 0f);
                }
                spriteBatch.Draw(texture, drawLocation, frame, color, rotation, origin, scale, SpriteEffects.None, 0f);
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

            System.Single opacity = particle.Opacity;
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

    }
}
