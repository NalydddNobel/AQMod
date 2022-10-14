using Microsoft.Xna.Framework;

namespace Aequus.Particles
{
    public class MonoParticle : BaseParticle
    {
        public MonoParticle(Vector2 position, Vector2 velocity, Color color = default(Color), float scale = 1f, float rotation = 0f) : base(position, velocity, color, scale, rotation)
        {
            SetTexture(ParticleTextures.monoParticle);
        }
    }
}