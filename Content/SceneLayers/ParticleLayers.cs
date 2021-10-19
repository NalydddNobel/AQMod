using AQMod.Assets.SceneLayers;
using AQMod.Content.Particles;
using System.Collections.Generic;

namespace AQMod.Content.SceneLayers
{
    public static class ParticleLayers
    {
        public static void AddParticle_PostDrawPlayers(Particle particle)
        {
            ParticleLayer_PostDrawPlayers.AddParticle(particle);
        }

        public static void UpdateParticles(List<Particle> particles)
        {
            for (int i = 0; i < particles.Count; i++)
            {
                if (!particles[i].Update())
                {
                    particles.RemoveAt(i);
                    i--;
                }
            }
        }

        public static void DrawParticles(List<Particle> particles)
        {
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Draw();
            }
        }
    }

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