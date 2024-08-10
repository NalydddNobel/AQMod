using Aequus.Common.Particles;
using Terraria.Graphics.Renderers;

namespace Aequus.Content.Items.Materials.OmniGem;

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
        Velocity *= 0.9f;
        float velo = Velocity.Length();
        Rotation += velo * 0.0314f;
        if (fadeIn > Scale) {
            Scale += 0.05f;
        }
        else {
            fadeIn = -1f;
            Scale -= 0.05f - velo / 1000f;
        }
        if (Scale <= 0.1f || float.IsNaN(Scale)) {
            RestInPool();
            return;
        }
        if (!dontEmitLight)
            Lighting.AddLight(Position, BloomColor.ToVector3() * 0.5f);
        Position += Velocity;
    }
}