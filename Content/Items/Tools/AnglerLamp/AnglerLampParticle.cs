using Aequus.Common.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria;

namespace Aequus.Content.Items.Tools.AnglerLamp;

public class AnglerLampParticle : BaseParticle<AnglerLampParticle> {
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
}