using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Renderers;

namespace Aequus.Particles
{
    public class BloomParticle : MonoParticle
    {
        public Color BloomColor;
        public float BloomScale = 1f;

        public Texture2D bloomTexture;
        public Vector2 bloomOrigin;

        public BloomParticle(Vector2 position, Vector2 velocity, Color color = default(Color), float scale = 1f, float bloomScale = 1f, float rotation = 0f) : base(position, velocity, color, scale, rotation)
        {
            BloomScale = bloomScale;
            SetTexture(ParticleTextures.monoParticle);
            bloomTexture = TextureCache.Bloom[0].Value;
            bloomOrigin = TextureCache.Bloom[0].Size() / 2f;
        }

        public BloomParticle(Vector2 position, Vector2 velocity, Color color = default(Color), Color bloomColor = default(Color), float scale = 1f, float bloomScale = 1f, float rotation = 0f) : this(position, velocity, color, scale, bloomScale, rotation)
        {
            BloomColor = bloomColor;
        }

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {
            spritebatch.Draw(bloomTexture, Position - Main.screenPosition, null, BloomColor, Rotation, bloomOrigin, Scale * BloomScale, SpriteEffects.None, 0f);
            base.Draw(ref settings, spritebatch);
        }
    }
}