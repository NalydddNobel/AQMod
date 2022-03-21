using System.Collections.Generic;

namespace AQMod.Effects.Particles
{
    public class ParticleSystem
    {
        public readonly ParticleLayer<ParticleType> PreDrawProjectiles;
        public readonly ParticleLayer<ParticleType> PostDrawPlayers;

        public ParticleSystem()
        {
            PreDrawProjectiles = new ParticleLayer<ParticleType>();
            PostDrawPlayers = new ParticleLayer<ParticleType>();
        }

        internal void UpdateParticles<T>(List<T> particles) where T : ParticleType
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

        internal void DrawParticles<T>(List<T> particles) where T : ParticleType
        {
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Draw();
            }
        }
    }
}