using Aequus.Core.Graphics;
using Aequus.Core.Particles;
using System;

namespace Aequus.Content.Tools.AnglerLamp;

public class AnglerLampParticles : ParticleSystem<AnglerLampParticles.Particle> {
    public override Int32 ParticleCount => 50;

    public override void Draw(SpriteBatch spriteBatch) {
        spriteBatch.BeginDusts();

        Texture2D texture = AequusTextures.FlareSoft;
        Rectangle frame = texture.Frame();
        Vector2 origin = frame.Size() / 2f;
        lock (this) {
            for (Int32 k = 0; k < Particles.Length; k++) {
                Particle particle = Particles[k];

                if (particle == null || !particle.Active) {
                    continue;
                }

                Single animation = particle.Animation;
                Vector2 drawLocation = particle.Location - Main.screenPosition;
                Color color = particle.Color with { A = 100 } * 0.8f;
                Color whiteColor = Color.White with { A = 0 } * Math.Min(animation, 1f);
                Single rotation = particle.Rotation;
                Single scale = particle.Scale * 0.66f;

                if (animation < 4f) {
                    var backDrawPosition = drawLocation + Main.rand.NextVector2Square(-4f + animation, 4f - animation) * 0.5f;
                    var backScale = new Vector2(scale * (4f - animation) * 0.3f, scale);
                    spriteBatch.Draw(texture, backDrawPosition, frame, color, rotation, origin, backScale, SpriteEffects.None, 0f);
                    spriteBatch.Draw(texture, backDrawPosition, frame, color, rotation + MathHelper.PiOver2, origin, backScale, SpriteEffects.None, 0f);
                }

                spriteBatch.Draw(texture, drawLocation, frame, whiteColor, rotation, origin, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(texture, drawLocation, frame, whiteColor, rotation + MathHelper.PiOver2, origin, scale, SpriteEffects.None, 0f);
            }
        }

        spriteBatch.End();
    }

    public override void Update() {
        for (Int32 i = 0; i < Particles.Length; i++) {
            Particle particle = Particles[i];
            if (particle == null || !particle.Active) {
                continue;
            }
            Active = true;

            particle.Animation += 0.05f;
            particle.Rotation += 0.3f * particle.Scale;
            if (particle.Animation < 4f) {
                particle.Scale *= 0.96f;

                if (particle.NPCAnchor != -1) {
                    if (particle.NPCOffset == Vector2.Zero) {
                        particle.NPCOffset = Main.npc[particle.NPCAnchor].Center - particle.Location;
                    }
                    if (particle.Animation < 2f) {
                        particle.Location = Vector2.Lerp(particle.Location, Main.npc[particle.NPCAnchor].Center + particle.NPCOffset, 1f - particle.Animation / 2f);
                    }
                }

                var d = Dust.NewDustPerfect(particle.Location, DustID.Torch, Alpha: 150, Scale: particle.Scale * 5f);
                d.noGravity = true;
                d.velocity *= 2f;
                continue;
            }

            particle.Scale -= 0.05f;
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
        private Boolean _active;
        public Boolean Active {
            get => _active;
            set {
                if (value) {
                    Rotation = Main.rand.NextFloat(-0.05f, 0.05f);
                    NPCAnchor = -1;
                    Animation = 0f;
                    Opacity = 1f;
                }
                _active = value;
            }
        }

        public Int32 NPCAnchor;
        public Vector2 NPCOffset;
        public Vector2 Location;

        public Single Rotation;

        public Single Scale;

        public Single Opacity;

        public Color Color;

        public Single Animation;
    }

    /*
    public float Animation;
    public int npc = -1;
    public Vector2 npcOffset;

    protected override void SetDefaults() {
        SetTexture(AequusTextures.Flare2);
        Rotation = Main.rand.NextFloat(-0.05f, 0.05f);
        Animation = 0f;
        npc = -1;
    }

    public override void Update(ref ParticleRendererSettings settings) {
        Animation += 0.05f;
        Rotation += 0.3f * Scale;
        if (Animation < 4f) {
            Scale *= 0.96f;

            if (npc != -1) {
                if (npcOffset == Vector2.Zero) {
                    npcOffset = Main.npc[npc].Center - Position;
                }
                if (Animation < 2f) {
                    Position = Vector2.Lerp(Position, Main.npc[npc].Center + npcOffset, 1f - Animation / 2f);
                }
            }

            var d = Dust.NewDustPerfect(Position, DustID.Torch, Alpha: 150, Scale: Scale * 5f);
            d.noGravity = true;
            d.velocity *= 2f;
            return;
        }
        base.Update(ref settings);
    }

    public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch) {
        var color = GetParticleColor(ref settings);
        var drawCoordinates = Position - Main.screenPosition;
        if (Animation < 4f) {
            var backDrawPosition = drawCoordinates + Main.rand.NextVector2Square(-4f + Animation, 4f - Animation) * 0.5f;
            var scale = new Vector2(Scale * (4f - Animation) * 0.3f, Scale);
            spritebatch.Draw(texture, backDrawPosition, frame, color with { A = 100 } * 0.5f, Rotation, origin, scale, SpriteEffects.None, 0f);
            spritebatch.Draw(texture, backDrawPosition, frame, color with { A = 100 } * 0.5f, Rotation + MathHelper.PiOver2, origin, scale, SpriteEffects.None, 0f);
        }
        spritebatch.Draw(texture, drawCoordinates, frame, Color.White with { A = 0 } * Math.Min(Animation, 1f), Rotation, origin, Scale, SpriteEffects.None, 0f);
    }
    */
}