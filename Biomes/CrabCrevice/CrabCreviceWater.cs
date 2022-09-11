using Aequus.Particles.Dusts;
using Terraria.ModLoader;

namespace Aequus.Biomes.CrabCrevice
{
    public class CrabCreviceWater : ModWaterStyle
    {
        public override int ChooseWaterfallStyle()
        {
            return ModContent.GetInstance<CrabCreviceWaterfall>().Slot;
        }

        public override int GetDropletGore()
        {
            return ModContent.GoreType<CrabCreviceDroplet>();
        }

        public override int GetSplashDust()
        {
            return ModContent.DustType<CrabCreviceSplash>();
        }
    }
}