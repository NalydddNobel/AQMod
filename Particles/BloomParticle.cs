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

        public Vector2 bloomOrigin;

        public virtual Texture2D BloomTexture => Images.Bloom[0].Value;

        public BloomParticle(Vector2 position, Vector2 velocity, Color color = default(Color), float scale = 1f, float bloomScale = 1f, float rotation = 0f) : base(position, velocity, color, scale, rotation)
        {
            BloomScale = bloomScale;
        }

        public BloomParticle(Vector2 position, Vector2 velocity, Color color = default(Color), Color bloomColor = default(Color), float scale = 1f, float bloomScale = 1f, float rotation = 0f) : this(position, velocity, color, scale, bloomScale, rotation)
        {
            BloomColor = bloomColor;
        }

        public override void OnAdd()
        {
            base.OnAdd();
            bloomOrigin = BloomTexture.Size() / 2f;
        }

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {
            DrawBloom(ref settings, spritebatch);
            base.Draw(ref settings, spritebatch);
        }

        public virtual void DrawBloom(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {
            spritebatch.Draw(BloomTexture, Position - Main.screenPosition, null, BloomColor, Rotation, bloomOrigin, Scale * BloomScale, SpriteEffects.None, 0f);
        }
    }
}