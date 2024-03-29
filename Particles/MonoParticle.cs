﻿using Aequus.Common.Particles;

namespace Aequus.Particles {
    public sealed class MonoParticle : BaseParticle<MonoParticle> {
        protected override void SetDefaults() {
            SetFramedTexture(AequusTextures.BaseParticleTexture, 3);
        }
    }
}