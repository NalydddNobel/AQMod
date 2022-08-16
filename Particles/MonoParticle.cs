using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Renderers;
using Terraria.ModLoader;

namespace Aequus.Particles
{
    public class MonoParticle : IParticle
    {
        private static Texture2D texture;
        public static Asset<Texture2D> ParticleTexture { get; private set; }
        public static Rectangle ParticleFrame { get; private set; }
        public static Vector2 ParticleOrigin { get; private set; }

        private class Loader : ILoadable
        {
            void ILoadable.Load(Mod mod)
            {
                ParticleTexture = ModContent.Request<Texture2D>(Aequus.AssetsPath + "Particles/Particle", AssetRequestMode.ImmediateLoad);
                texture = ParticleTexture.Value;
                ParticleFrame = ParticleTexture.Frame(verticalFrames: 3, frameY: 0);
                ParticleOrigin = ParticleFrame.Size() / 2f;
            }

            void ILoadable.Unload()
            {
                ParticleTexture = null;
                texture = null;
            }
        }

        public Vector2 Position;
        public Vector2 Velocity;
        public Color Color;
        public float Scale;
        public float Rotation;

        public Rectangle frame;
        public Vector2 origin;

        public virtual Texture2D Texture => texture;
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
            frame = ParticleFrame;
            frame.Y = frame.Height * Main.rand.Next(3);
            origin = ParticleOrigin;
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