using Aequu2.Core.Graphics;
using Aequu2.Core.Particles;

namespace Aequu2.Old.Content.StatusEffects;

public sealed class BlueFireSparkle : ParallelParticleArray<BlueFireSparkle.Particle> {
    public override int ParticleCount => 1000;

    public override void Draw(SpriteBatch spriteBatch) {
        spriteBatch.BeginDusts();

        Texture2D texture = Aequu2Textures.BlueFireSparkle;
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
                spriteBatch.Draw(texture, drawLocation, frame, particle.Color * scale, rotation, origin, scale, SpriteEffects.None, 0f);
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

            particle.Velocity *= 0.8f;
            if (particle.animation < 0) {
                particle.Scale -= particle.Scale / 30f;
            }
            else {
                particle.Scale += 0.035f;
            }

            if (particle.animation < 0 && particle.Scale < 0.5f && Main.rand.NextBool(60)) {
                particle.animation = Main.rand.Next(20);
            }
            particle.animation--;
            if (particle.Scale <= 0.1f || float.IsNaN(particle.Scale)) {
                particle.Active = false;
                return;
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

        public int animation;
    }
}