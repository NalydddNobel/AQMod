using Aequus.Graphics.Primitives;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Renderers;

namespace Aequus.Particles
{
    public class StormcloakTrailParticle : BoundBowTrailParticle
    {
        public float rotationValue = 0f;

        public StormcloakTrailParticle(TrailRenderer prim, Vector2 position, Vector2 velocity, Color color = default(Color), float scale = 1, float rotation = 0, int trailLength = 10, bool drawDust = true) : base(prim, position, velocity, color, scale, rotation, trailLength, drawDust)
        {
        }

        public override void Update(ref ParticleRendererSettings settings)
        {
            Velocity = Velocity.RotatedBy(rotationValue) * 0.97f;
            float velo = Velocity.Length();
            Rotation += 0.03f;
            Scale -= 0.05f - velo / 1000f;
            if (Scale <= 0.1f)
            {
                ShouldBeRemovedFromRenderer = true;
                return;
            }
            Lighting.AddLight(Position, Color.ToVector3() * 0.5f);
            Position += Velocity;
            oldPos[0] = Position;
            AequusHelpers.UpdateCacheList(oldPos);
        }
    }
}