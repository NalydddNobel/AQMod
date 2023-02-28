using Aequus.Common.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Renderers;

namespace Aequus.Particles
{
    public sealed class StormcloakTrailParticle : BaseTrailParticle<StormcloakTrailParticle>
    {
        public TrailRenderer prim;
        public float rotationValue = 0f;

        public override StormcloakTrailParticle CreateInstance()
        {
            return new StormcloakTrailParticle();
        }

        public StormcloakTrailParticle Setup(TrailRenderer prim, Vector2 position, Vector2 velocity, Color color = default(Color), float scale = 1, float rotation = 0, int trailLength = 10)
        {
            this.prim = prim;
            if (oldPos == null)
                oldPos = new Vector2[10];
            Setup(position, velocity, trailLength, color, scale, rotation);
            return this;
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
            if (!dontEmitLight)
                Lighting.AddLight(Position, Color.ToVector3() * 0.5f);
            Position += Velocity;
            oldPos[0] = Position;
            Helper.UpdateCacheList(oldPos);
        }

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {
            prim.Draw(oldPos);
        }
    }
}