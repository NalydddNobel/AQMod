using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Particles.Dusts {
    public class CosmicCrystalDust : ModDust {
        public override void OnSpawn(Dust dust) {
            dust.noGravity = true;
            dust.noLight = true;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor) {
            return new Color(255 - dust.alpha, 255 - dust.alpha, 255 - dust.alpha, 255 - dust.alpha);
        }
    }
}