﻿using Aequus.Common.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Renderers;

namespace Aequus.Particles {
    public sealed class MonoFlashParticle : BaseBloomParticle<MonoFlashParticle> {
        public int flash;

        protected override void SetDefaults() {
            SetFramedTexture(AequusTextures.Flare, 1);
            bloomOrigin = AequusTextures.Bloom0.Size() / 2f;
            flash = 0;
        }

        public override void Update(ref ParticleRendererSettings settings) {
            base.Update(ref settings);
            flash++;
            if (flash < 7) {
                Scale *= 1.2f;
            }
            else {
                Scale *= 0.9f;
            }
        }

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch) {
            float globalScale = 0.45f;
            spritebatch.Draw(texture, Position - Main.screenPosition, frame, Color, Rotation, origin, new Vector2(Scale * 0.5f, Scale) * globalScale, SpriteEffects.None, 0f);
            spritebatch.Draw(texture, Position - Main.screenPosition, frame, Color, Rotation + MathHelper.PiOver2, origin, new Vector2(Scale * 0.5f, Scale) * globalScale, SpriteEffects.None, 0f);
            spritebatch.Draw(AequusTextures.Bloom0, Position - Main.screenPosition, null, BloomColor * Scale, Rotation, bloomOrigin, Scale * BloomScale * globalScale, SpriteEffects.None, 0f);
        }
    }
}