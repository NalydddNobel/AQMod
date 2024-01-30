using Aequus.Core.Graphics;
using Aequus.Core.Particles;
using System;

namespace Aequus.Content.Weapons.Magic.Furystar;

public class FurystarParticles : ParticleArray<FurystarParticles.Particle> {
    public override int ParticleCount => 50;

    public override void Draw(SpriteBatch spriteBatch) {
        spriteBatch.BeginDusts();

        Texture2D texture = AequusTextures.FlareSoft;
        Rectangle frame = texture.Frame();
        Vector2 origin = frame.Size() / 2f;

        Texture2D lensFlare = AequusTextures.LensFlare;
        Rectangle lensFlareFrame = lensFlare.Frame();
        Vector2 lensFlareOrigin = lensFlareFrame.Size() / 2f;
        lock (this) {
            for (int k = 0; k < Particles.Length; k++) {
                Particle particle = Particles[k];

                if (particle == null || !particle.Active) {
                    continue;
                }


                float rotation = particle.Rotation;
                float scale = particle.Scale;
                Vector2 drawLocation = particle.Location - Main.screenPosition;
                Color color = particle.Color;

                if (particle.Animation < 4f) {
                    spriteBatch.Draw(texture, drawLocation + Main.rand.NextVector2Square(-4f + particle.Animation, 4f - particle.Animation) * 0.5f, frame, color with { A = 20 }, rotation, origin, new Vector2(scale * (4f - particle.Animation) * 1.3f, particle.Scale * 1.3f), SpriteEffects.None, 0f);
                    spriteBatch.Draw(lensFlare, drawLocation, lensFlareFrame, Color.White * 0.2f, rotation * 3f, AequusTextures.LensFlare.Size() / 2f, scale * 2.5f, SpriteEffects.None, 0f);
                }
                spriteBatch.Draw(texture, drawLocation, frame, Color.White with { A = 0 } * Math.Min(particle.Animation, 1f), rotation, origin, scale, SpriteEffects.None, 0f);
            }
        }

        spriteBatch.End();
    }

    public override void Update() {
        for (int i = 0; i < Particles.Length; i++) {
            Particle particle = Particles[i];
            if (particle == null || !particle.Active) {
                continue;
            }
            Active = true;

            particle.Animation += 0.4f;
            if (particle.Animation < 4f) {
                particle.Scale *= 0.85f;
                var d = Dust.NewDustPerfect(particle.Location, DustID.ManaRegeneration, Alpha: 150, Scale: particle.Scale * 5f);
                d.noGravity = true;
                d.velocity *= 2f;
                continue;
            }

            particle.Location += particle.Velocity;
            particle.Velocity *= 0.9f;
            particle.Scale -= 0.05f;
            particle.Rotation += 0.5f * particle.Scale;

            if (particle.Scale <= 0f) {
                particle.Active = false;
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
        private bool _active;
        public bool Active {
            get => _active;
            set {
                if (value) {
                    Rotation = Main.rand.NextFloat(-0.05f, 0.05f);
                    Animation = 0f;
                }
                _active = value;
            }
        }

        public Vector2 Location;
        public Vector2 Velocity;

        public float Rotation;

        public float Scale;

        public Color Color;

        public float Animation;
    }
}