using Aequus.Common.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Renderers;

namespace Aequus.Content.Items.Material.OmniGem;

public class OmniGemParticle : BaseBloomParticle<OmniGemParticle> {
    private float fadeIn;

    protected override void SetDefaults() {
        SetFramedTexture(AequusTextures.BaseParticleTexture, 3);
        fadeIn = 0f;
    }

    public override void Update(ref ParticleRendererSettings settings) {
        if (fadeIn == 0f) {
            fadeIn = Scale + 0.9f;
        }
        Velocity *= 0.92f;
        float speed = Velocity.Length();
        Rotation += Scale * 0.01f * Math.Sign(Velocity.X);
        if (fadeIn > Scale) {
            Scale += 0.03f + Scale * 0.1f;
        }
        else {
            fadeIn = -1f;
            Scale -= 0.05f + speed / 1000f + Scale * 0.01f;
        }
        if (Scale <= 0.1f || float.IsNaN(Scale)) {
            RestInPool();
            return;
        }
        if (!dontEmitLight) {
            Lighting.AddLight(Position, BloomColor.ToVector3() * 0.5f);
        }

        Position += Velocity;
    }

    public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch) {
        spritebatch.Draw(AequusTextures.Bloom, Position - Main.screenPosition, null, BloomColor, Rotation, bloomOrigin, Scale * BloomScale, SpriteEffects.None, 0f);
        spritebatch.Draw(AequusTextures.Flare, Position - Main.screenPosition, null, Color, Rotation, AequusTextures.Flare.Size() / 2f, new Vector2(0.15f, 0.3f) * Scale, SpriteEffects.None, 0f);
        spritebatch.Draw(AequusTextures.Flare, Position - Main.screenPosition, null, Color, Rotation + MathHelper.PiOver2, AequusTextures.Flare.Size() / 2f, new Vector2(0.2f, 0.4f) * Scale, SpriteEffects.None, 0f);
    }
}