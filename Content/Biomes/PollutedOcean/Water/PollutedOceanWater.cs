using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Aequus.Content.Biomes.PollutedOcean.Water;

public class PollutedOceanWater : ModWaterStyle {
    public override int ChooseWaterfallStyle() {
        return ModContent.GetInstance<PollutedOceanWaterfall>().Slot;
    }

    public override int GetDropletGore() {
        return ModContent.GoreType<PollutedOceanDroplet>();
    }

    public override int GetSplashDust() {
        return ModContent.DustType<PollutedOceanSplash>();
    }

    public override Color BiomeHairColor() {
        return Color.Teal;
    }
}