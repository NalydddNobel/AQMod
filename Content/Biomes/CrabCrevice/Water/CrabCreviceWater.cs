#if !CRAB_CREVICE_DISABLE
using Aequus.Particles.Dusts;

namespace Aequus.Content.Biomes.CrabCrevice.Water;
public class CrabCreviceWater : ModWaterStyle {
    public override int ChooseWaterfallStyle() {
        return ModContent.GetInstance<CrabCreviceWaterfall>().Slot;
    }

    public override int GetDropletGore() {
        return ModContent.GoreType<CrabCreviceDroplet>();
    }

    public override int GetSplashDust() {
        return ModContent.DustType<CrabCreviceSplash>();
    }

    public override Color BiomeHairColor() {
        return Color.Teal;
    }
}
#endif