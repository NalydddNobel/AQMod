using Aequus.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Renderers;

namespace Aequus.Particles
{
    public class BloomParticle : MonoParticle, IPooledParticle
    {
        public Color BloomColor;
        public float BloomScale = 1f;

        public Texture2D bloomTexture;
        public Vector2 bloomOrigin;

        public BloomParticle() : base(Vector2.Zero, Vector2.Zero)
        {
        }

        [Obsolete("Use BloomParticle.New(...) instead")]
        public BloomParticle(Vector2 position, Vector2 velocity, Color color = default(Color), float scale = 1f, float bloomScale = 1f, float rotation = 0f) : base(position, velocity, color, scale, rotation)
        {
            BloomScale = bloomScale;
            SetTexture(ParticleTextures.monoParticle);
            bloomTexture = Textures.Bloom[0].Value;
            bloomOrigin = Textures.Bloom[0].Size() / 2f;
        }

        [Obsolete("Use BloomParticle.New(...) instead")]
        public BloomParticle(Vector2 position, Vector2 velocity, Color color = default(Color), Color bloomColor = default(Color), float scale = 1f, float bloomScale = 1f, float rotation = 0f) : this(position, velocity, color, scale, bloomScale, rotation)
        {
            BloomColor = bloomColor;
        }

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {
            spritebatch.Draw(bloomTexture, Position - Main.screenPosition, null, BloomColor, Rotation, bloomOrigin, Scale * BloomScale, SpriteEffects.None, 0f);
            base.Draw(ref settings, spritebatch);
        }

        public void Setup(Vector2 position, Vector2 velocity, Color color = default(Color), Color bloomColor = default(Color), float scale = 1f, float bloomScale = 1f, float rotation = 0f)
        {
            Position = position;
            Velocity = velocity;
            Color = color;
            Scale = scale;
            Rotation = rotation;
            BloomColor = bloomColor;
            BloomScale = bloomScale;
            dontEmitLight = false;
            SetTexture(ParticleTextures.monoParticle);
            bloomTexture = Textures.Bloom[0].Value;
            bloomOrigin = Textures.Bloom[0].Size() / 2f;
        }

        public static BloomParticle New(Vector2 position, Vector2 velocity, Color color = default(Color), Color bloomColor = default(Color), float scale = 1f, float bloomScale = 1f, float rotation = 0f)
        {
            var particle = EffectsSystem.BloomPool.RequestParticle();
            particle.Setup(position, velocity, color, bloomColor, scale, bloomScale, rotation);
            return particle;
        }
    }
}