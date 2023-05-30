namespace Aequus.Particles {
    public sealed class BloomParticle : BaseBloomParticle<BloomParticle>
    {
        public override BloomParticle CreateInstance()
        {
            return new BloomParticle();
        }

        protected override void SetDefaults()
        {
            SetTexture(ParticleTextures.monoParticle);
        }
    }
}