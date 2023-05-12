using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Renderers;

namespace Aequus.Particles
{
    public class DashBlurParticle : BaseParticle<DashBlurParticle>
    {
        public float animation;
        /// <summary>
        /// Defaults to 0.5f.
        /// </summary>
        public float ScaleX;

        public override DashBlurParticle CreateInstance()
        {
            return new();
        }

        protected override void SetDefaults()
        {
            SetTexture(ParticleTextures.shinyFlashParticle, 1);
            animation = 0f;
            ScaleX = 0.5f;
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
                new Vector2(ScaleX, Scale), SpriteEffects.None, 0f);
        }
    }
}