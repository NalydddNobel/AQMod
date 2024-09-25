using Aequus.Common.Drawing;
using Aequus.Common.Particles.New;
using Aequus.Particles.Dusts;
using System;

namespace Aequus.Content.Tiles.Herbs;

public class HerbBreakParticles : ParticleArray<HerbBreakParticles.Particle> {
    public override int ParticleCount => 100;

    public override void Draw(SpriteBatch spriteBatch) {
        Texture2D texture = AequusTextures.SeaPicklesParticle;

        spriteBatch.Begin_Dusts();
        for (int i = 0; i < Particles.Length; i++) {
            var p = Particles[i];
            if (p == null || !p.Active) {
                continue;
            }

            Rectangle frame = texture.Frame(horizontalFrames: 2, verticalFrames: 3, frameX: 0, frameY: i % 3);
            Vector2 origin = frame.Size();
            Vector2 drawCoordinates = p.Location - Main.screenPosition;
            spriteBatch.DrawAlign(AequusTextures.Bloom, drawCoordinates, null, p.Color with { A = 0 } * 0.05f * p.Opacity, 0f, 1f, SpriteEffects.None);
            spriteBatch.DrawAlign(texture, drawCoordinates, frame, p.Color with { A = 0 } * p.Opacity, 0f, 1f, SpriteEffects.None);
            if (p.Opacity > 0.5f) {
                float opacity = (p.Opacity - 0.5f) / 0.5f;
                spriteBatch.DrawAlign(AequusTextures.FlareSoft, drawCoordinates, null, p.Color with { A = 0 } * 0.15f * opacity, 0f, 1f, SpriteEffects.None);
            }
        }
        spriteBatch.End();
    }

    public override void Update() {
        for (int i = 0; i < Particles.Length; i++) {
            var p = Particles[i];

            if (p == null || !p.Active) {
                continue;
            }

            p.Location += p.Velocity;

            p.Velocity.X *= 0.8f;
            if (p.Velocity.Y > 0f) {
                p.Velocity *= 0.8f;
            }
            if (Main.rand.NextBool(7)) {
                p.Velocity.X += Main.rand.NextFloat(-0.75f, 0.75f);
            }
            Dust d = Dust.NewDustPerfect(p.Location, ModContent.DustType<MonoDust>(), p.Velocity * 0.2f, newColor: p.Color with { A = 0 } * p.Opacity);
            p.Velocity.Y -= Main.rand.NextFloat(0.12f);
            if (Math.Abs(p.Velocity.X) < 0.1f) {
                p.Opacity -= 0.03f;
            }
            if (p.Opacity <= 0f) {
                p.Active = false;
            }
            else {
                Active = true;
            }
        }
    }

    public override void Activate() {
        Instance<DrawLayers>().PostDrawDust += Draw;
    }

    public override void Deactivate() {
        Instance<DrawLayers>().PostDrawDust -= Draw;
    }

    public class Particle : IParticle {
        public bool Active { get; set; }
        public Vector2 Location;
        public Vector2 Velocity;
        public Color Color;
        public float Opacity;
    }
}
