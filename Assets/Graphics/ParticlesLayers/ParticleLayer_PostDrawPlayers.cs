using AQMod.Assets.Graphics.Particles;
using System.Collections.Generic;

namespace AQMod.Assets.Graphics.ParticlesLayers
{
    public class ParticleLayer_PostDrawPlayers : SceneLayer
    {
        private static List<Particle> _particles;

        public ParticleLayer_PostDrawPlayers()
        {
            _particles = new List<Particle>();
        }

        public static void AddParticle(Particle particle)
        {
            particle.OnAdd();
            _particles.Add(particle);
        }

        public override void Update()
        {
            ParticleLayers.UpdateParticles(_particles);
        }

        protected override void Draw()
        {
            ParticleLayers.DrawParticles(_particles);
        }
    }
}
