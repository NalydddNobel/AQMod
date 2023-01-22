using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Renderers;

namespace Aequus.Particles
{
    public class ShinyFlashParticle : BloomParticle
    {
        public int flash;

        public ShinyFlashParticle(Vector2 position, Vector2 velocity, Color color = default(Color), float scale = 1f, float bloomScale = 1f, float rotation = 0f) : base(position, velocity, color, scale, rotation)
        {
            BloomScale = bloomScale;
            SetTexture(ParticleTextures.shinyFlashParticle, 1);
            bloomTexture = TextureCache.Bloom[0].Value;
            bloomOrigin = TextureCache.Bloom[0].Value.Size() / 2f;
        }

        public ShinyFlashParticle(Vector2 position, Vector2 velocity, Color color = default(Color), Color bloomColor = default(Color), float scale = 1f, float bloomScale = 1f, float rotation = 0f) : this(position, velocity, color, scale, bloomScale, rotation)
        {
            BloomColor = bloomColor;
        }

        public override void Update(ref ParticleRendererSettings settings)
        {
            base.Update(ref settings);
            flash++;
            if (flash < 10)
            {
                Scale *= 1.12f;
            }
            else
            {
                Scale *= 0.99f;
            }
        }

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {
            float globalScale = 0.75f;
            spritebatch.Draw(texture, Position - Main.screenPosition, frame, Color, Rotation, origin, new Vector2(Scale * 0.5f, Scale) * globalScale, SpriteEffects.None, 0f);
            spritebatch.Draw(texture, Position - Main.screenPosition, frame, Color, Rotation + MathHelper.PiOver2, origin, new Vector2(Scale * 0.5f, Scale) * globalScale, SpriteEffects.None, 0f);
            spritebatch.Draw(bloomTexture, Position - Main.screenPosition, null, BloomColor * Scale, Rotation, bloomOrigin, Scale * BloomScale * globalScale, SpriteEffects.None, 0f);
        }
    }
}