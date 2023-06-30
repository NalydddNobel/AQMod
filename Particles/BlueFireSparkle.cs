using Aequus.Common.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Renderers;

namespace Aequus.Particles {
    public sealed class BlueFireSparkle : BaseBloomParticle<BlueFireSparkle> {
        public int animation;

        protected override void SetDefaults() {
            SetFramedTexture(AequusTextures.BlueFireSparkle, 3);
        }

        public override Color GetParticleColor(ref ParticleRendererSettings settings) {
            return base.GetParticleColor(ref settings) * Scale;
        }

        public override void Update(ref ParticleRendererSettings settings) {
            Velocity *= 0.8f;
            if (animation < 0) {
                Scale -= Scale / 30f;
            }
            else {
                Scale += 0.035f;
            }
            animation--;
            if (Scale <= 0.1f || float.IsNaN(Scale)) {
                RestInPool();
                return;
            }
            if (!dontEmitLight)
                Lighting.AddLight(Position, Color.ToVector3() * 0.5f);
            Position += Velocity;
        }
    }
}