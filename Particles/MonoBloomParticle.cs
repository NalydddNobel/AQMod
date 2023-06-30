using Aequus.Common.Particles;

namespace Aequus.Particles {
    public sealed class MonoBloomParticle : BaseBloomParticle<MonoBloomParticle> {
        protected override void SetDefaults() {
            SetFramedTexture(AequusTextures.BaseParticleTexture, 3);
        }
    }
}