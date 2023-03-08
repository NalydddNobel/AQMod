using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Renderers;

namespace Aequus.Particles
{
    public class DashBlurParticle : BaseParticle<DashBlurParticle>
    {
        public float animation;

        public override DashBlurParticle CreateInstance()
        {
            return new();
        }

        protected override void SetDefaults()
        {
            SetTexture(ParticleTextures.shinyFlashParticle, 1);
            animation = 0f;
        }

        public override void Update(ref ParticleRendererSettings settings)
        {
            if (animation > 10)
            {
                RestInPool();
                return;
            }

            if (!dontEmitLight)
                Lighting.AddLight(Position, Color.ToVector3() * 0.5f);
            Position += Velocity;

            animation++;
        }

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {
            spritebatch.Draw(
                texture, 
                Position - Main.screenPosition, 
                frame, 
                GetParticleColor(ref settings) * ((10f - animation) / 10f), 
                Rotation, 
                origin, 
                new Vector2(0.5f, Scale), SpriteEffects.None, 0f);
        }
    }
}