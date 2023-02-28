using Microsoft.Xna.Framework;
using Terraria.Graphics.Renderers;

namespace Aequus.Particles
{
    public abstract class BaseTrailParticle<T> : BaseParticle<T> where T : BaseTrailParticle<T>
    {
        public Vector2[] oldPos;

        public void Setup(Vector2 position, Vector2 velocity, int trailLength, Color color = default(Color), float scale = 1f, float rotation = 0f)
        {
            Position = position;
            Velocity = velocity;
            Color = color;
            Scale = scale;
            Rotation = rotation;
            SetTrailLength(trailLength);
            SetDefaults();
        }

        public void SetTrailLength(int trailLength)
        {
            oldPos = new Vector2[trailLength];
        }

        public override void Update(ref ParticleRendererSettings settings)
        {
            base.Update(ref settings);
            if (!ShouldBeRemovedFromRenderer)
            {
                oldPos[0] = Position;
                Helper.UpdateCacheList(oldPos);
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