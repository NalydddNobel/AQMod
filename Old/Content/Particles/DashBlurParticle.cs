using Aequu2.Core.Graphics;
using Aequu2.Core.Particles;
using System;

namespace Aequu2.Old.Content.Particles;

public class DashBlurParticle : ParallelParticleArray<DashBlurParticle.Particle> {
    public override int ParticleCount => 2000;

    public override void Draw(SpriteBatch spriteBatch) {
        spriteBatch.BeginDusts();

        Texture2D texture = Aequu2Textures.FlareSoft;
        Rectangle frame = texture.Frame();
        Vector2 origin = frame.Size() / 2f;
        lock (this) {
            for (int k = 0; k < Particles.Length; k++) {
                Particle particle = Particles[k];

                if (particle == null || !particle.Active) {
                    continue;
                }

                float rotation = particle.Rotation + MathHelper.PiOver2;
                Vector2 drawLocation = particle.Location - Main.screenPosition;
                Vector2 velocity = particle.Velocity;

                Color color = particle.Color;
                float alpha = Math.Clamp(1f - (10f - particle.animation) / 10f, 0f, 1f);
                float r = color.R - 255 * alpha;
                float g = color.G - 255 * alpha;
                float b = color.B - 255 * alpha;
                float a = color.A - 255 * alpha;
                Color finalColor = new Color(Math.Max((int)r, 0), Math.Max((int)g, 0), Math.Max((int)b, 0), Math.Max((int)a, 0));
                Vector2 finalScale = new Vector2(particle.ScaleX, particle.Scale);
                spriteBatch.Draw(texture, drawLocation, frame, finalColor, rotation, origin, finalScale, SpriteEffects.None, 0f);
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

            if (particle.animation > 10) {
                particle.Active = false;
                return;
            }

            if (Collision.SolidCollision(particle.Location, 2, 2)) {
                particle.animation++;
                particle.animation = Math.Max(particle.animation, 0);
                return;
            }
            if (!particle.dontEmitLight) {
                Lighting.AddLight(particle.Location, particle.Color.ToVector3() * 0.5f);
            }

            particle.Location += particle.Velocity;

            particle.animation++;
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
                    ScaleX = 0.5f;
                    animation = 0f;
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

        public float animation;
        /// <summary>
        /// Defaults to 0.5f.
        /// </summary>
        public float ScaleX;
    }
}