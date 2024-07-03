using Aequu2.Core.Graphics;
using Aequu2.Core.Particles;

namespace Aequu2.Old.Content.Items.Materials.OmniGem;

public class OmniGemParticle : ParallelParticleArray<OmniGemParticle.Particle> {
    public override int ParticleCount => 2000;

    public override void Draw(SpriteBatch spriteBatch) {
        spriteBatch.BeginDusts();

        Texture2D texture = Aequu2Textures.BaseParticleTexture;
        Texture2D bloomStrongTexture = Aequu2Textures.BloomStrong;
        Texture2D bloomTexture = Aequu2Textures.Bloom;
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
                float bloomScale = particle.BloomScale * scale;
                Vector2 drawLocation = particle.Location - Main.screenPosition;
                Vector2 velocity = particle.Velocity;

                spriteBatch.Draw(bloomStrongTexture, drawLocation, null, particle.BloomColor * scale * 0.1f, rotation, Aequu2Textures.BloomStrong.Size() / 2f, bloomScale * 3f, SpriteEffects.None, 0f);
                spriteBatch.Draw(bloomTexture, drawLocation, null, particle.BloomColor * scale, rotation, bloomOrigin, bloomScale, SpriteEffects.None, 0f);
                spriteBatch.Draw(texture, drawLocation, frame, particle.Color * scale, rotation, origin, scale, SpriteEffects.None, 0f);

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

            if (particle.FadeIn == 0f) {
                particle.FadeIn = particle.Scale + 0.9f;
            }
            particle.Velocity *= 0.92f;
            float speed = particle.Velocity.Length();
            if (particle.FadeIn > particle.Scale) {
                particle.Scale += 0.04f + particle.Scale * 0.1f;
            }
            else {
                particle.FadeIn = -1f;
                particle.Scale -= 0.05f + speed / 1000f + particle.Scale * 0.01f;
            }
            if (particle.Scale <= 0.1f || float.IsNaN(particle.Scale)) {
                particle.Active = false;
                return;
            }
            if (!particle.dontEmitLight) {
                Lighting.AddLight(particle.Location, particle.BloomColor.ToVector3() * 0.5f);
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
                    FadeIn = 0f;
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

        public float FadeIn;
    }
}