using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Renderers;

namespace Aequus.Particles
{
    public class MonoParticle : IParticle
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Color Color;
        public float Scale;
        public float Rotation;

        public Rectangle frame;
        public Vector2 origin;

        public virtual Texture2D Texture => TextureCache.Particle.Value;
        public virtual Color ParticleColor => Color;

        public bool ShouldBeRemovedFromRenderer { get; private set; }

        public MonoParticle(Vector2 position, Vector2 velocity, Color color = default(Color), float scale = 1f, float rotation = 0f)
        {
            Position = position;
            Velocity = velocity;
            Color = color;
            Scale = scale;
            Rotation = rotation;
            OnAdd();
        }

        public virtual void OnAdd()
        {
            frame = new Rectangle(0, 10 * Main.rand.Next(3), 10, 10);
            origin = frame.Size() / 2f;
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
            spritebatch.Draw(Texture, Position - Main.screenPosition, frame, ParticleColor, Rotation, origin, Scale, SpriteEffects.None, 0f);
        }
    }
}