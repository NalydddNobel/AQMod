using System.Collections.Generic;

namespace AQMod.Common.Graphics.Particles.Rendering
{
    public sealed class ParticleRenderer<TParticle> where TParticle : ParticleType
    {
        internal readonly List<TParticle> _particles;

        public ParticleRenderer()
        {
            _particles = new List<TParticle>();
        }

        public void AddParticle(TParticle p)
        {
            _particles.Add(p);
        }
    }
}