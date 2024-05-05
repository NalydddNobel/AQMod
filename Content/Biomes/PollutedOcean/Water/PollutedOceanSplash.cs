using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.Biomes.PollutedOcean.Water;

public class PollutedOceanSplash : ModDust {
    public override void SetStaticDefaults() {
        UpdateType = 33;
    }

    public override void OnSpawn(Terraria.Dust dust) {
        dust.alpha = 170;
        dust.velocity *= 0.5f;
        dust.velocity.Y += 1f;
    }
}