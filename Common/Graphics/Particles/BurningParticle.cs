using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Renderers;
using Terraria.ModLoader;

namespace AQMod.Common.Graphics.Particles
{
    public class BurningParticle : ABasicParticle
    {
        private static Asset<Texture2D> _mainTexture;
        private float scale;
        private Color color;

        public BurningParticle(Vector2 position, Vector2 velocity = default(Vector2), Color color = default(Color), float scale = 1f)
        {
            FetchFromPool();
            LocalPosition = position;
            Velocity = velocity;
            this.color = color;
            this.scale = scale;
            ShouldBeRemovedFromRenderer = false;
        }

        public override void FetchFromPool()
        {
            base.FetchFromPool();
            _frame = new Rectangle(0, 30 * Main.rand.Next(3), 30, 30);
            _origin = _frame.Size() / 2f;
        }

        public override void Update(ref ParticleRendererSettings settings)
        {
            Velocity *= 0.9f;
            float velo = Velocity.Length();
            Rotation += velo * 0.0314f;
            scale -= 0.05f - velo / 1000f;
            if (scale <= 0.1f)
            {
                ShouldBeRemovedFromRenderer = true;
            }
            LocalPosition += Velocity;
            Lighting.AddLight(LocalPosition, color.ToVector3() * 0.5f);
        }

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {
            if (_mainTexture == null)
            {
                _mainTexture = ModContent.Request<Texture2D>("AQMod/Assets/Particles/BurningParticle");
            }
            else if (_mainTexture.Value != null)
            {
                Main.spriteBatch.Draw(_mainTexture.Value, LocalPosition - Main.screenPosition, _frame, color, Rotation, _origin, scale, SpriteEffects.None, 0f);
            }
        }
    }
}
