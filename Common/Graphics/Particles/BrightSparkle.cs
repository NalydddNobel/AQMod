using AQMod.Assets;
using AQMod.Effects.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace AQMod.Common.Graphics.Particles
{
    public class BrightSparkle : ParticleType
    {
        public Rectangle frame;
        public Vector2 origin;
        public Vector2 velocity;
        public Vector2 position;
        public float scale;
        public float rotation;
        public Color color;

        public BrightSparkle(Vector2 position, Vector2 velocity = default(Vector2), Color color = default(Color), float scale = 1f)
        {
            this.position = position;
            this.velocity = velocity;
            this.color = color;
            this.scale = scale;
        }

        public override void OnAdd()
        {
            frame = new Rectangle(0, 0, 29, 29);
            origin = frame.Size() / 2f;
        }

        public override bool Update()
        {
            velocity *= 0.9f;
            float velo = velocity.Length();
            scale -= 0.05f - velo / 1000f;
            if (scale <= 0.1f)
                return false;
            Lighting.AddLight(position, color.ToVector3() * 0.5f);
            position += velocity;
            return true;
        }

        public override void Draw()
        {
            var texture = AQTextures.Particles[ParticleTex.BrightSparkle];
            Main.spriteBatch.Draw(texture, position - Main.screenPosition, frame, color * 0.8f, rotation, origin, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, position - Main.screenPosition, frame, color * 0.3f, rotation + MathHelper.PiOver4, origin, scale * 0.8f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, position - Main.screenPosition, frame, new Color(255, 255, 255, 255), rotation, origin, scale * 0.75f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, position - Main.screenPosition, frame, new Color(128, 128, 128, 128), rotation + MathHelper.PiOver4, origin, scale * 0.5f, SpriteEffects.None, 0f);
        }
    }
}