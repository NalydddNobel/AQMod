using AQMod.Assets;
using AQMod.Common.ID;
using AQMod.Effects.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace AQMod.Content.Seasonal.Christmas
{
    public class CloseBGSnowflake : ParticleType
    {
        public Rectangle frame;
        public Vector2 origin;
        public Vector2 velocity;
        public Vector2 position;
        public float scale;
        public float rotation;
        public Color color;
        private ushort timeLeft;

        public CloseBGSnowflake(Vector2 position, Vector2 velocity = default(Vector2), Color color = default(Color), float scale = 1f)
        {
            this.position = position;
            this.velocity = velocity;
            this.color = color;
            this.scale = scale;
            timeLeft = 600;
        }

        public override void OnAdd()
        {
            frame = new Rectangle(0, 0, 22, 22);
            origin = frame.Size() / 2f;
        }

        public override bool Update()
        {
            velocity.X = MathHelper.Lerp(velocity.X, XmasSeeds.snowflakeWind, 0.01f) + (float)Math.Sin(Main.GlobalTime + position.Y / 32f) * 0.02f;
            if (velocity.Y < 4f)
                velocity.Y += 0.05f;
            float velo = velocity.Length();
            scale -= 0.001f - velo / 10000f;
            rotation += scale * 0.2f;
            if (timeLeft > 0)
                timeLeft--;
            else
            {
                scale *= 0.9f;
            }
            if (scale <= 0.1f)
                return false;
            position += velocity;
            return true;
        }

        public override void Draw()
        {
            var texture = LegacyTextureCache.Particles[ParticleTex.SpaceSquidSnowflake];
            Main.spriteBatch.Draw(texture, position - Main.screenPosition, frame, new Color(255, 255, 255, 255), rotation, origin, scale, SpriteEffects.None, 0f);
        }
    }
}