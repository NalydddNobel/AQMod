using Microsoft.Xna.Framework;
using Terraria.Graphics.Renderers;

namespace Aequus.Particles
{
    public abstract class BaseTrailParticle : MonoParticle
    {
        public Vector2[] oldPos;

        public BaseTrailParticle(Vector2 position, Vector2 velocity, Color color = default(Color), float scale = 1f, float rotation = 0f, int trailLength = 10) : base(position, velocity, color, scale, rotation)
        {
            oldPos = new Vector2[trailLength];
        }

        public override void Update(ref ParticleRendererSettings settings)
        {
            base.Update(ref settings);
            if (!ShouldBeRemovedFromRenderer)
            {
                oldPos[0] = Position;
                AequusHelpers.UpdateCacheList(oldPos);
            }
        }

        public void StretchTrail(Vector2 direction)
        {
            oldPos[0] = Position;
            for (int i = 1; i < oldPos.Length; i++)
            {
                oldPos[i] = oldPos[i - 1] + direction * i;
            }
        }
    }
}