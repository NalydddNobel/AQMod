using AQMod.Content.Particles;
using System.Collections.Generic;

namespace AQMod.Assets.Graphics.ParticlesLayers
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
}