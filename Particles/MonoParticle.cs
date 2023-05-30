namespace Aequus.Particles {
    public sealed class MonoParticle : BaseParticle<MonoParticle>
    {
        protected override void SetDefaults()
        {
            SetTexture(ParticleTextures.monoParticle);
        }

        public override MonoParticle CreateInstance()
        {
            return new MonoParticle();
        }
    }
}