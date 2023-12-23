using Aequus.Common.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Graphics.Renderers;

namespace Aequus.Content.Weapons.Magic.Furystar;

public class FurystarParticle : BaseParticle<FurystarParticle> {
    public float Animation;

    protected override void SetDefaults() {
        SetTexture(AequusTextures.Flare2);
        Rotation = Main.rand.NextFloat(-0.05f, 0.05f);
        Animation = 0f;
    }

    public override void Update(ref ParticleRendererSettings settings) {
        Animation += 0.66f;
        if (Animation < 4f) {
            Scale *= 0.85f;
            var d = Dust.NewDustPerfect(Position, DustID.ManaRegeneration, Alpha: 150, Scale: Scale * 5f);
            d.noGravity = true;
            d.velocity *= 2f;
            return;
        }
        base.Update(ref settings);
        Rotation += 0.5f * Scale;
    }

    public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch) {
        var color = GetParticleColor(ref settings);
        var drawCoordinates = Position - Main.screenPosition;
        if (Animation < 4f) {
            spritebatch.Draw(texture, drawCoordinates + Main.rand.NextVector2Square(-4f + Animation, 4f - Animation) * 0.5f, frame, color with { A = 20 }, Rotation, origin, new Vector2(Scale * (4f - Animation) * 1.3f, Scale * 1.3f), SpriteEffects.None, 0f);
        }
        spritebatch.Draw(texture, drawCoordinates, frame, Color.White with { A = 0 } * Math.Min(Animation, 1f), Rotation, origin, Scale, SpriteEffects.None, 0f);
    }
}