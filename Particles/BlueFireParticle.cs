using Aequus.Common.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Renderers;

namespace Aequus.Particles {
    public sealed class BlueFireParticle : BaseBloomParticle<BlueFireParticle> {
        protected override void SetDefaults() {
            SetFramedTexture(AequusTextures.BlueFireParticle, 3);
        }

        public override void Update(ref ParticleRendererSettings settings) {
            base.Update(ref settings);
            if (Main.rand.NextBool(50)) {
                var sparkle = ParticleSystem.New<BlueFireSparkle>(ParticleLayer.BehindPlayers).Setup(
                    Position,
                    Velocity,
                    new Color(255, 255, 255, 0) * 0.35f,
                    Main.rand.NextFloat(0.2f, 0.8f)
                );
                sparkle.animation = Main.rand.Next(40);
            }
        }
    }
}
