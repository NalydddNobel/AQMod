using Aequus.Graphics.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Renderers;

namespace Aequus.Particles
{
    public class BoundBowTrailParticle : BaseTrailParticle
    {
        public TrailRenderer prim;
        public bool drawDust;

        public BoundBowTrailParticle(TrailRenderer prim, Vector2 position, Vector2 velocity, Color color = default(Color), float scale = 1, float rotation = 0, int trailLength = 10, bool drawDust = true) : base(position, velocity, color, scale, rotation, trailLength)
        {
            this.prim = prim;
            this.drawDust = drawDust;
        }

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {
            prim.Draw(oldPos);
            if (!drawDust)
                base.Draw(ref settings, spritebatch);
        }
    }
}