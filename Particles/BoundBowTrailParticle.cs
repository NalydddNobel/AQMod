using Aequus.Graphics.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Renderers;

namespace Aequus.Particles
{
    public sealed class BoundBowTrailParticle : BaseTrailParticle<BoundBowTrailParticle>
    {
        public TrailRenderer prim;
        public bool drawDust;

        public override BoundBowTrailParticle CreateInstance()
        {
            return new BoundBowTrailParticle();
        }

        protected override void SetDefaults()
        {
            if (oldPos == null)
                oldPos = new Vector2[10];
            SetTexture(ParticleTextures.monoParticle);
        }

        public BoundBowTrailParticle Setup(TrailRenderer prim, Vector2 position, Vector2 velocity, Color color = default(Color), float scale = 1, float rotation = 0, int trailLength = 10, bool drawDust = true)
        {
            this.prim = prim;
            this.drawDust = drawDust;
            Setup(position, velocity, trailLength, color, scale, rotation);
            return this;
        }

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {
            prim.Draw(oldPos);
            if (!drawDust)
                base.Draw(ref settings, spritebatch);
        }
    }
}