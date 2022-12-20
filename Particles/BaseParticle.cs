using Aequus.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Renderers;

namespace Aequus.Particles
{
    public abstract class BaseParticle : IParticle
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Color Color;
        public float Scale;
        public float Rotation;

        public Texture2D texture;
        public Rectangle frame;
        public Vector2 origin;

        public virtual Color GetParticleColor(ref ParticleRendererSettings settings)
        {
            return Color;
        }

        public bool ShouldBeRemovedFromRenderer { get; protected set; }

        public BaseParticle(Vector2 position, Vector2 velocity, Color color = default(Color), float scale = 1f, float rotation = 0f)
        {
            Position = position;
            Velocity = velocity;
            Color = color;
            Scale = scale;
            Rotation = rotation;
        }
        
        protected void SetTexture(TextureInfo textureInfo, int frames = 3)
        {
            texture = textureInfo.Texture.Value;
            frame = textureInfo.Frame;
            frame.Y = frame.Height * Main.rand.Next(frames);
            origin = textureInfo.Origin;
        }

        public virtual void Update(ref ParticleRendererSettings settings)
        {
            Velocity *= 0.9f;
            float velo = Velocity.Length();
            Rotation += velo * 0.0314f;
            Scale -= 0.05f - velo / 1000f;
            if (Scale <= 0.1f)
            {
                ShouldBeRemovedFromRenderer = true;
                return;
            }
            Lighting.AddLight(Position, Color.ToVector3() * 0.5f);
            Position += Velocity;
        }

        public virtual void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {
            spritebatch.Draw(texture, Position - Main.screenPosition, frame, GetParticleColor(ref settings), Rotation, origin, Scale, SpriteEffects.None, 0f);
        }
    }
}