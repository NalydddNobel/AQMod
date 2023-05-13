using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Renderers;

namespace Aequus.Particles {
    public class DashBlurParticle : BaseParticle<DashBlurParticle> {
        public float animation;
        /// <summary>
        /// Defaults to 0.5f.
        /// </summary>
        public float ScaleX;

        public override DashBlurParticle CreateInstance() {
            return new();
        }

        protected override void SetDefaults() {
            SetTexture(ParticleTextures.shinyFlashParticle, 1);
            animation = 0f;
            ScaleX = 0.5f;
        }

        public override void Update(ref ParticleRendererSettings settings) {
            if (animation > 10) {
                RestInPool();
                return;
            }

            if (Collision.SolidCollision(Position, 2, 2)) {
                animation++;
                animation = Math.Max(animation, 0);
                return;
            }
            if (!dontEmitLight)
                Lighting.AddLight(Position, Color.ToVector3() * 0.5f);
            Position += Velocity;

            animation++;
        }

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch) {
            var clr = GetParticleColor(ref settings);
            float alpha = Math.Clamp(1f - ((10f - animation) / 10f), 0f, 1f);
            float r = clr.R - 255 * alpha;
            float g = clr.G - 255 * alpha;
            float b = clr.B - 255 * alpha;
            float a = clr.A - 255 * alpha;
            spritebatch.Draw(
                texture,
                Position - Main.screenPosition,
                frame,
                new Color(Math.Max((int)r, 0), Math.Max((int)g, 0), Math.Max((int)b, 0), Math.Max((int)a, 0)),
                Rotation,
                origin,
                new Vector2(ScaleX, Scale), SpriteEffects.None, 0f);
        }
    }
}