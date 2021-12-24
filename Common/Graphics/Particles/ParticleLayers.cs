using AQMod.Common.Graphics.SceneLayers;
using System.Collections.Generic;

namespace AQMod.Common.Graphics.Particles
{
    public static class ParticleLayers
    {
        private abstract class particlelayer : SceneLayer
        {
            private List<Particle> _particles;

            protected sealed override void OnRegister(LayerKey key)
            {
                _particles = new List<Particle>();
                setupKey(key);
            }

            protected abstract void setupKey(LayerKey key);

            public sealed override string Name => "Particles_" + Layering.ToString();
            public sealed override SceneLayering Layering => SceneLayering.PostDrawPlayers;

            public void AddParticle(Particle particle)
            {
                particle.OnAdd();
                _particles.Add(particle);
            }

            public sealed override void Update()
            {
                UpdateParticles(_particles);
            }

            protected sealed override void Draw()
            {
                DrawParticles(_particles);
            }
        }

        private class ParticleLayer_PostDrawPlayers : particlelayer
        {
            public static LayerKey Key { get; private set; }

            protected override void setupKey(LayerKey key)
            {
                Key = key;
            }
        }

        public static void AddParticle_PostDrawPlayers(Particle particle)
        {
            ParticleLayer_PostDrawPlayers.Key.GetLayer<ParticleLayer_PostDrawPlayers>().AddParticle(particle);
        }

        internal static void UpdateParticles<T>(List<T> particles) where T : Particle
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

        internal static void DrawParticles<T>(List<T> particles) where T : Particle
        {
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Draw();
            }
        }
    }
}