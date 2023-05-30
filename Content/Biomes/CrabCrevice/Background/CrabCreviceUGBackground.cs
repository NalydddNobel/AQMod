using Terraria.ModLoader;

namespace Aequus.Content.Biomes.CrabCrevice.Background {
    public class CrabCreviceUGBackground : ModUndergroundBackgroundStyle
    {
        public override void FillTextureArray(int[] textureSlots)
        {
            textureSlots[0] = 290;
            textureSlots[1] = 291;
            textureSlots[2] = 291;
            textureSlots[3] = 297;
        }
    }
}