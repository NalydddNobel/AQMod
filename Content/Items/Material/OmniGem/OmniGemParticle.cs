using Aequus.Common.Particles;
using Microsoft.Xna.Framework.Graphics;
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
        if (fadeIn > Scale) {
            Scale += 0.04f + Scale * 0.1f;
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
        spritebatch.Draw(AequusTextures.BloomStrong, Position - Main.screenPosition, null, BloomColor * Scale * 0.1f, Rotation, AequusTextures.BloomStrong.Size() / 2f, Scale * BloomScale * 3f, SpriteEffects.None, 0f);
        spritebatch.Draw(AequusTextures.Bloom, Position - Main.screenPosition, null, BloomColor * Scale, Rotation, bloomOrigin, Scale * BloomScale, SpriteEffects.None, 0f);
        spritebatch.Draw(texture, Position - Main.screenPosition, frame, Color * Scale, Rotation, origin, Scale, SpriteEffects.None, 0f);
    }
}