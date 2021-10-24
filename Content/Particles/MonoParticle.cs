using AQMod.Assets;
using AQMod.Assets.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace AQMod.Content.Particles
{
    public class MonoParticle : Particle
    {
        public Rectangle frame;
        public Vector2 origin;
        public Vector2 velocity;
        public Vector2 position;
        public float scale;
        public float rotation;
        public Color color;

        protected virtual Texture2D Texture => TextureCache.Particles[ParticleTextureID.Mono];
        protected virtual Color GetColor() => color;

        public MonoParticle(Vector2 position, Vector2 velocity = default(Vector2), Color color = default(Color), float scale = 1f)
        {
            this.position = position;
            this.velocity = velocity;
            this.color = color;
            this.scale = scale;
        }

        public override void OnAdd()
        {
            frame = new Rectangle(0, 10 * Main.rand.Next(3), 10, 10);
            origin = frame.Size() / 2f;
        }

        public override bool Update()
        {
            velocity *= 0.9f;
            float velo = velocity.Length();
            rotation += velo * 0.0314f;
            scale -= 0.05f - velo / 1000f;
            if (scale <= 0.1f)
                return false;
            Lighting.AddLight(position, color.ToVector3() * 0.5f);
            position += velocity;
            return true;
        }

        public override void Draw()
        {
            var texture = Texture;
            var color = GetColor();
            Main.spriteBatch.Draw(texture, position - Main.screenPosition, frame, color, rotation, origin, scale, SpriteEffects.None, 0f);
        }
    }
}