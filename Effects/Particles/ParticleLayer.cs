using AQMod.Common.Graphics.Particles;
using System.Collections.Generic;

namespace AQMod.Effects.Particles
{
    public sealed class ParticleLayer<TParticle> where TParticle : ParticleType
    {
        internal List<TParticle> _particles;

        public ParticleLayer()
        {
            Initialize();
        }

        public void AddParticle(TParticle p)
        {
            p.OnAdd();
            _particles.Add(p);
        }

        public void Initialize()
        {
            _particles = new List<TParticle>();
        }

        public void UpdateParticles()
        {
            Particle.UpdateParticles(_particles);
        }

        public void Render()
        {
            Particle.DrawParticles(_particles);
        }
    }
}