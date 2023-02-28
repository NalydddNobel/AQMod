using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Renderers;

namespace Aequus.Particles
{
    public sealed class RadonAmbientParticle : BaseBloomParticle<RadonAmbientParticle>
    {
        public float fadeIn;

        public override RadonAmbientParticle CreateInstance()
        {
            return new RadonAmbientParticle();
        }

        protected override void SetDefaults()
        {
            SetTexture(ParticleTextures.monoParticle);
            fadeIn = Scale + 0.33f;
        }

        public override void Update(ref ParticleRendererSettings settings)
        {
            if (Scale < fadeIn)
            {
                Scale += 0.01f;
                if (Scale >= fadeIn)
                {
                    fadeIn = 0f;
                }
                Position += Velocity;
                return;
            }
            base.Update(ref settings);
        }

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {
            float opacity = fadeIn != 0 ? -(Scale - fadeIn) / 0.33f : 1f;
            if (opacity <= 0f)
                return;
            if (opacity > 1f)
                opacity = 1f;
            spritebatch.Draw(bloomTexture, Position - Main.screenPosition, null, BloomColor * opacity, Rotation, bloomOrigin, Scale * BloomScale, SpriteEffects.None, 0f);
            spritebatch.Draw(texture, Position - Main.screenPosition, frame, GetParticleColor(ref settings) * opacity, Rotation, origin, Scale, SpriteEffects.None, 0f);
        }
    }
}